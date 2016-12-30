using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Events;

namespace com.kintoshmalae.SFXEngine.Audio.Sources {
    class SilenceSoundFX : SoundFX {

        public SilenceSoundFX(TimeSpan length) {
            this.length = length;
            this.canSeek = true;
            this.canDuplicate = true;
        }

        public SilenceSoundFX(uint sampleLength) : this(new TimeSpan((sampleLength * TimeSpan.TicksPerSecond) / AudioSampleFormat.DefaultFormat.samplesPerSecond)) {}

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

        protected override SoundFX basicDup {
            get {
                return new SilenceSoundFX(sampleLength);
            }
        }

        protected override UInt32 readSample(Single[] buffer, UInt32 offset, UInt32 count) {
            var samplesAvailable = sampleLength - _currentSample;
            var samplesRead = (uint)Math.Min(samplesAvailable, count);
            readSilence(buffer, offset, samplesRead);
            return samplesRead;
        }
    }
}
