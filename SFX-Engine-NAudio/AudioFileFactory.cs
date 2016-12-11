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

        public AudioFileFactory() : base(
            new[] { AudioDataType.MP3, AudioDataType.WAVE },        // types readable from file
            new[] { AudioDataType.MP3 }                             // types readable from memory
            ){}

        public override SoundFX loadMP3(byte[] audioData) {
            return new MP3MemorySoundFX(audioData);
        }

        public override SoundFX loadAudio(String fName) {
            throw new NotImplementedException();
        }
    }
}
