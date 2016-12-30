using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Audio;

namespace com.kintoshmalae.SFXEngine.Events {
    public enum PlaybackEvent {
        Play, Stop, Sample, Pause, Resume, Reset, Seek
    }

    public class SoundEventArgs : EventBaseArgs<SoundFX> {
        public PlaybackEvent EventType { get; private set; }
        public uint CurrentSample { get; private set; }

        public SoundEventArgs(SoundFX source, PlaybackEvent type, uint sample) : base(source) {
            this.EventType = type;
            this.CurrentSample = sample;
        }

        public override String ToString() {
            return EventType + ":[" + CurrentSample + "]";
        }
    }
}
