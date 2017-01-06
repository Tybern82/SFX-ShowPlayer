using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Events;
using com.kintoshmalae.SFXEngine.I18N;

namespace com.kintoshmalae.SFXEngine.Audio.Sources {
    public class CachedSoundFX : SoundFX {

        private float[] audioData;

        public CachedSoundFX(float[] audioData) {
            if (audioData == null) audioData = new float[0];
            this.audioData = audioData;
            this.canDuplicate = true;
            this.canSeek = true;
        }

        public CachedSoundFX(SoundFX source) : this(new float[0]) {
            // TODO: Check maximum length of cacheable source
            if (source == null) throw new ArgumentNullException(I18NString.Lookup("NullPointer"));
            this.audioFormat = source.audioFormat;
            lock (_lock) {
                List<float> audio = new List<float>();
                float[] buffer = new float[source.audioFormat.channelCount];
                while (source.currentState != PlaybackMode.Stop) {
                    int read = (int)source.read(buffer, 0, (uint)buffer.Length);
                    audio.AddRange(buffer.Take(read));
                }
                audioData = audio.ToArray();
            }
        }

        protected override SoundFX basicDup {
            get {
                return new CachedSoundFX(audioData);
            }
        }

        public override Boolean seekTo(UInt32 samplePosition) {
            if (samplePosition > sampleLength) return false;
            this._currentSample = samplePosition;
            seekToFade();
            onSeek.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Seek, samplePosition));
            return true;
        }

        public override Boolean seekForward(UInt32 samples) {
            return seekTo(_currentSample + samples);
        }

        protected override UInt32 readSample(Single[] buffer, UInt32 offset, UInt32 count) {
            var availableSamples = audioData.Length - _currentSample;
            var samplesRead = (uint)Math.Min(count, availableSamples);
            Array.Copy(audioData, _currentSample, buffer, offset, samplesRead);
            return samplesRead;
        }
    }
}
