using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Audio;
using com.kintoshmalae.SFXEngine.Exceptions;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace com.kintoshmalae.SFXEngine.NAudio {
    public class AudioPlaybackEngine : com.kintoshmalae.SFXEngine.Audio.AudioPlaybackEngine, IDisposable {
        public AudioSampleFormat audioFormat { get; private set; } = AudioSampleFormat.DefaultFormat;

        private bool isClosed = false;
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;

        private IDictionary<SoundFX, ISampleProvider> runningFX = new Dictionary<SoundFX, ISampleProvider>();
        private object _lock = new object();

        public AudioPlaybackEngine() : this(AudioSampleFormat.DefaultSampleRate, AudioSampleFormat.DefaultChannelCount) { }

        public AudioPlaybackEngine(uint sampleRate, uint channels) {
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
                outputDevice = new WaveOutEvent();
                mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat((int)audioFormat.sampleRate, (int)audioFormat.channelCount));
                mixer.ReadFully = true;
                outputDevice.Init(mixer);
                outputDevice.Play();
            }
        }

        ~AudioPlaybackEngine() {
            if (!isClosed) Dispose();
        }

        public void play(SoundFX fx) {
            lock (_lock) {
                if (fx == null) return;
                if (fx is ISampleProvider) {
                    play(fx as ISampleProvider);
                } else {
                    ISampleProvider source = new SoundFXSampleProvider(fx);
                    runningFX.Add(fx, source);
                    fx.onStop.onTrigger += OnStop_onTrigger;
                    play(source);
                }
            }
        }

        private void OnStop_onTrigger(SoundFX source, Events.EventBaseArgs<SoundFX> args) {
            lock (_lock) {
                if (source == null) return;
                if (runningFX.ContainsKey(source)) runningFX.Remove(source);
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
                if (fx is ISampleProvider) {
                    stop(fx as ISampleProvider);
                } else {
                    ISampleProvider source = runningFX.ContainsKey(fx) ? runningFX[fx] : null;
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
            lock (_lock) {
                isClosed = true;
                outputDevice.Dispose();
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
