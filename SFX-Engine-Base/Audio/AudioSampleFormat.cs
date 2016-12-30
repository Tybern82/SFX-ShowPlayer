using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.Audio {
    public class AudioSampleFormat {
        public static readonly uint DefaultSampleRate = 44100;
        public static readonly uint DefaultChannelCount = 2;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] // AudioSampleFormat is Immutable
        public static readonly AudioSampleFormat DefaultFormat = new AudioSampleFormat(DefaultSampleRate, DefaultChannelCount);

        public uint sampleRate { get; }
        public uint channelCount { get; }

        public uint samplesPerSecond { get { return sampleRate * channelCount; } }

        public AudioSampleFormat(uint sampleRate, uint channelCount) {
            this.sampleRate = sampleRate;
            this.channelCount = channelCount;
        }

        public static bool operator ==(AudioSampleFormat samp1, AudioSampleFormat samp2) {
            if (samp1 == null) return (samp2 == null);
            if (samp2 == null) return false;
            return (samp1.sampleRate == samp2.sampleRate) && (samp1.channelCount == samp2.channelCount);
        }

        public static bool operator !=(AudioSampleFormat samp1, AudioSampleFormat samp2) {
            return !(samp1 == samp2);
        }

        public override String ToString() {
            string _result = "";
            switch (channelCount) {
                case 6:
                    _result = "5.1";
                    break;

                case 8:
                    _result = "7.1";
                    break;

                default:
                    _result += channelCount;
                    break;
            }
            return _result + "@" + sampleRate + "hz";
        }

        public override Boolean Equals(Object obj) {
            AudioSampleFormat samp2 = obj as AudioSampleFormat;
            return (samp2 != null) ? (this == samp2) : false;
        }

        public override Int32 GetHashCode() {
            return ToString().GetHashCode();
        }
    }
}
