using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.Audio {
    public class AudioSampleFormat {
        public static readonly uint DefaultSampleRate = 44100;
        public static readonly uint DefaultChannels = 2;
        public static readonly AudioSampleFormat DefaultFormat = new AudioSampleFormat(DefaultSampleRate, DefaultChannels);

        public uint sampleRate { get; protected set; }
        public uint channels { get; protected set; }

        public uint samplesPerSecond { get { return sampleRate * channels; } }

        public AudioSampleFormat(uint sampleRate, uint channels) {
            this.sampleRate = sampleRate;
            this.channels = channels;
        }
    }
}
