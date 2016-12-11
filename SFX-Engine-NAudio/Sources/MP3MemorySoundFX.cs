using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Audio;
using com.kintoshmalae.SFXEngine.Events;
using NAudio.Wave;

namespace com.kintoshmalae.SFXEngine.NAudio.Sources {
    class MP3MemorySoundFX : SoundFX, IDisposable, ISampleProvider {

        private Mp3FileReader reader;
        private ISampleProvider source;
        private byte[] mp3Data;

        public MP3MemorySoundFX(Mp3FileReader reader) {
            if (reader == null) throw new ArgumentNullException();
            this.reader = reader;
            this.length = reader.TotalTime;
            this.source = reader.ToSampleProvider();
            this.canSeek = true;
            this.audioFormat = SFXUtilities.FromWaveFormat(reader.WaveFormat);
        }

        public MP3MemorySoundFX(byte[] mp3Data) : this(new Mp3FileReader(new MemoryStream(mp3Data))) {
            this.canDuplicate = true;
            this.mp3Data = mp3Data;
        }

        ~MP3MemorySoundFX() {
            if (reader != null) Dispose();
        }

        public WaveFormat WaveFormat {
            get {
                return SFXUtilities.ToWaveFormat(audioFormat);
            }
        }

        public void Dispose() {
            if (reader != null) reader.Dispose();
            reader = null;
        }

        public Int32 Read(Single[] buffer, Int32 offset, Int32 count) {
            return (int)read(buffer, (uint)offset, (uint)count);
        }

        public override TimeSpan currentTime {
            get {
                return reader.CurrentTime;
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
                reader.CurrentTime = index;
                // ... calculate the relative sample position,...
                _currentSample = (uint)Math.Round(index.TotalSeconds * audioFormat.samplesPerSecond, MidpointRounding.ToEven);
                // .... and trigger the seek event
                onSeek.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Seek, _currentSample));
                return true;
            }
        }

        public override SoundFX dup() {
            return (canDuplicate ? new MP3MemorySoundFX(mp3Data) : base.dup());
        }

        protected override UInt32 readSample(Single[] buffer, UInt32 offset, UInt32 count) {
            return (uint)source.Read(buffer, (int)offset, (int)count);
        }
    }
}
