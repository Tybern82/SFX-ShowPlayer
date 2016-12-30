using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Audio;
using com.kintoshmalae.SFXEngine.Exceptions;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace com.kintoshmalae.SFXEngine.NAudio {
    public class AudioPlaybackEngine : Audio.IAudioPlaybackEngine, IDisposable {
        public AudioSampleFormat audioFormat { get; private set; } = AudioSampleFormat.DefaultFormat;

        private bool isClosed = false;
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;

        private IDictionary<SoundFX, ISampleProvider> runningFX = new Dictionary<SoundFX, ISampleProvider>();
        private object _lock = new object();

        public static string[] driverNames() {
            // Prefer ASIO drivers for output (low-latency), fallback to DirectSound if not present
            if (AsioOut.isSupported()) {
                return AsioOut.GetDriverNames();
            } else {
                var devs = DirectSoundOut.Devices;
                string[] _result = new string[devs.Count()];
                for (var x = 0; x < devs.Count(); x++) {
                    _result[x] = devs.ElementAt(x).ModuleName;
                }
                return _result;
            }
        }

        public static Audio.IAudioPlaybackEngine loadDevice(string devName) {
            // TODO: Update to cache the loaded devices and return the existing engine, rather than creating a new instance
            return new AudioPlaybackEngine(devName);
        }

        public static Audio.IAudioPlaybackEngine loadDevice(string devName, AudioSampleFormat audioFormat) {
            if (audioFormat == null) audioFormat = AudioSampleFormat.DefaultFormat;
            return new AudioPlaybackEngine(devName, audioFormat.sampleRate, audioFormat.channelCount);
        }

        private static Guid findDXDevice(string name) {
            foreach (var d in DirectSoundOut.Devices) {
                if (d.ModuleName == name) return d.Guid;
            }
            return Guid.Empty;
        }

        public AudioPlaybackEngine() : this(AudioSampleFormat.DefaultSampleRate, AudioSampleFormat.DefaultChannelCount) { }

        public AudioPlaybackEngine(string driverName) : this(driverName, AudioSampleFormat.DefaultSampleRate, AudioSampleFormat.DefaultChannelCount) {}

        public AudioPlaybackEngine(uint sampleRate, uint channels) : this(null, sampleRate, channels) {}

        public AudioPlaybackEngine(string driverName, uint sampleRate, uint channels) {
            lock (_lock) {
                this.audioFormat = new AudioSampleFormat(sampleRate, channels);
                switch (audioFormat.channelCount) {
                    case 1:
                    case 2:
                    case 6:
                    case 8:
                        break;
                    default:
                        throw new NotImplementedException("Only able to support mono, stereo, 5.1 and 7.1 channel outputs.");
                }
                if (driverName == null) {
                    if (AsioOut.isSupported()) {
                        outputDevice = new AsioOut();
                    } else {
                        outputDevice = new DirectSoundOut();
                    }
                } else {
                    if (AsioOut.isSupported()) {
                        outputDevice = new AsioOut(driverName);
                    } else {
                        var guid = findDXDevice(driverName);
                        outputDevice = (guid == Guid.Empty) ? new DirectSoundOut() : new DirectSoundOut(guid);
                    }
                }
                // outputDevice = new WaveOutEvent();
                mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat((int)audioFormat.sampleRate, (int)audioFormat.channelCount));
                mixer.ReadFully = true;
                outputDevice.Init(mixer);
                outputDevice.Play();
            }
        }

        ~AudioPlaybackEngine() {
            Dispose(false);
        }

        public void play(SoundFX fx) {
            lock (_lock) {
                if (fx == null) return;
                ISampleProvider source = fx as ISampleProvider;
                if (source != null) { 
                    play(source);
                } else {
                    source = new SoundFXSampleProvider(fx);
                    runningFX.Add(fx, source);
                    fx.onStop.onTrigger += OnStop_onTrigger;
                    play(source);
                }
            }
        }

        private void OnStop_onTrigger(object sender, Events.EventBaseArgs<SoundFX> args) {
            lock (_lock) {
                if (args == null) return;
                if (args.Source == null) return;
                if (runningFX.ContainsKey(args.Source)) runningFX.Remove(args.Source);
            }
        }

        public void stop() {
            lock (_lock) {
                mixer.RemoveAllMixerInputs();
                runningFX.Clear();
            }
        }

        public void stop(SoundFX fx) {
            lock (_lock) {
                if (fx == null) return;
                ISampleProvider source = fx as ISampleProvider;
                if (source != null) { 
                    stop(source);
                } else {
                    source = runningFX.ContainsKey(fx) ? runningFX[fx] : null;
                    stop(source);
                    if (runningFX.ContainsKey(fx)) runningFX.Remove(fx);
                }
            }
        }

        public void stop(ISampleProvider snd) {
            lock (_lock) {
                if (snd == null) return;
                mixer.RemoveMixerInput(snd);
            }
        }

        public ISampleProvider play(ISampleProvider source) {
            lock (_lock) {
                try {
                    source = SFXUtilities.ConvertSampleFormat(source, mixer.WaveFormat);
                    mixer.AddMixerInput(source);
                    return source;
                } catch (UnsupportedAudioException) {
                    return null;
                }
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool managed) {
            lock (_lock) {
                if (!isClosed) {
                    isClosed = true;
                    outputDevice.Dispose();
                }
                if (managed) {
                    runningFX = null;
                    audioFormat = null;
                }
            }
        }
    }

    class SoundFXSampleProvider : ISampleProvider {
        private readonly SoundFX fx;

        public SoundFXSampleProvider(SoundFX fx) {
            this.fx = fx;
            this.WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int)fx.audioFormat.sampleRate, (int)fx.audioFormat.channelCount);
        }

        public WaveFormat WaveFormat { get; private set; }

        public Int32 Read(Single[] buffer, Int32 offset, Int32 count) {
            return (int)fx.read(buffer, (uint)offset, (uint)count);
        }
    }
}
