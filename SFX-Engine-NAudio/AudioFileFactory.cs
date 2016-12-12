using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Audio;
using com.kintoshmalae.SFXEngine.Exceptions;
using com.kintoshmalae.SFXEngine.I18N;
using com.kintoshmalae.SFXEngine.NAudio.Sources;

namespace com.kintoshmalae.SFXEngine.NAudio {
    public class AudioFileFactory : com.kintoshmalae.SFXEngine.Audio.AudioFileFactory {

        private static AudioDataType[] Supported = new [] {
            AudioDataType.AIFF, AudioDataType.MP3, AudioDataType.WAVE, AudioDataType.FLAC, AudioDataType.OGG
        };

        public AudioFileFactory() : base(Supported, Supported) { }

        public override SoundFX loadMP3(byte[] audioData) {
            return new MP3MemorySoundFX(audioData);
        }

        public override SoundFX loadAudio(String fName) {
            throw new NotImplementedException();
        }
    }
}
