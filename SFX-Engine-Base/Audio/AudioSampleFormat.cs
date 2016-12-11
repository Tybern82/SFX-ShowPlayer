using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.Audio {
    public class AudioSampleFormat {
        public static readonly uint DefaultSampleRate = 44100;
        public static readonly uint DefaultChannelCount = 2;
        public static readonly AudioSampleFormat DefaultFormat = new AudioSampleFormat(DefaultSampleRate, DefaultChannelCount);

        public uint sampleRate { get; protected set; }
        public uint channelCount { get; protected set; }

        public uint samplesPerSecond { get { return sampleRate * channelCount; } }

        public AudioSampleFormat(uint sampleRate, uint channelCount) {
            this.sampleRate = sampleRate;
            this.channelCount = channelCount;
        }
    }
}
