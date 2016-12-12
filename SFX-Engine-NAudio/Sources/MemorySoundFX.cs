using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Audio;
using com.kintoshmalae.SFXEngine.Events;
using NAudio.Wave;

namespace com.kintoshmalae.SFXEngine.NAudio.Sources {
    abstract class MemorySoundFX : SoundFX, IDisposable, ISampleProvider {

        private WaveStream reader;
        private ISampleProvider source;

        protected MemorySoundFX(WaveStream reader) {
            lock (_lock) {
                if (reader == null) throw new ArgumentNullException();
                this.reader = reader;
                this.length = reader.TotalTime;
                this.source = reader.ToSampleProvider();
                this.canSeek = true;
                this.audioFormat = SFXUtilities.FromWaveFormat(reader.WaveFormat);
            }
        }

        ~MemorySoundFX() {
            if (reader != null) Dispose();
        }

        public void Dispose() {
            lock (_lock) {
                if (reader != null) reader.Dispose();
                reader = null;
            }
        }

        public WaveFormat WaveFormat {
            get {
                return SFXUtilities.ToWaveFormat(audioFormat);
            }
        }

        public Int32 Read(Single[] buffer, Int32 offset, Int32 count) {
            return (int)read(buffer, (uint)offset, (uint)count);
        }

        public override TimeSpan currentTime {
            get {
                lock (_lock) {
                    return (reader != null) ? reader.CurrentTime : TimeSpan.Zero;
                }
            }
        }

        public override Boolean seekForward(uint sampleLength) {
            return seekTo(sampleLength + currentSample);
        }

        public override Boolean seekForward(TimeSpan ts) {
            return seekTo(currentTime + ts);
        }

        public override Boolean seekTo(uint sampleIndex) {
            long millis = (long)Math.Round((((double)(sampleIndex * 1000)) / WaveFormat.Channels / WaveFormat.SampleRate), MidpointRounding.AwayFromZero);
            long secs = millis / 1000;
            long mins = secs / 60;
            long hours = mins / 60;
            int days = (int)(hours / 24);
            TimeSpan index = new TimeSpan(days, (int)(hours % 24), (int)(mins % 60), (int)(secs % 60), (int)(millis % 1000));
            return seekTo(index);
        }

        public override Boolean seekTo(TimeSpan index) {
            lock (_lock) {
                // Move the reader to the correct time index,...
                if (reader != null) reader.CurrentTime = index;
                // ... calculate the relative sample position,...
                _currentSample = (uint)Math.Round(index.TotalSeconds * audioFormat.samplesPerSecond, MidpointRounding.ToEven);
                // .... and trigger the seek event
                onSeek.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Seek, _currentSample));
                seekToFade();   // adjust for any fade control
                return true;
            }
        }

        protected abstract MemorySoundFX basicDup();

        public override SoundFX dup() {
            lock (_lock) {
                if (!canDuplicate) return base.dup();
                MemorySoundFX _result = basicDup();
                if (_result != null) {
                    _result.seekTo(this.currentSample);         // move to the same position as the current effect...
                    _result.dupFadeState(this);                 // ... copy the current fade conditions...
                    _result.currentState = this.currentState;   // ... and adjust to the same play state
                } else {
                    throw new ArgumentNullException();  // _result should NEVER be null
                }
                return _result;
            }
        }

        protected override UInt32 readSample(Single[] buffer, UInt32 offset, UInt32 count) {
            if (reader == null) return 0;   // reader closed, no more samples to read
            return (uint)source.Read(buffer, (int)offset, (int)count);
        }
    }
}
