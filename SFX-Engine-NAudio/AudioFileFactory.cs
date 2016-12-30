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

        public override SoundFX loadAIFF(Byte[] audioData) {
            return new AIFFMemorySoundFX(audioData);
        }

        public override SoundFX loadMP3(byte[] audioData) {
            return new MP3MemorySoundFX(audioData);
        }

        public override SoundFX loadWAVE(Byte[] audioData) {
            return new WAVEMemorySoundFX(audioData);
        }

        public override SoundFX loadFLAC(Byte[] audioData) {
            return new FLACMemorySoundFX(audioData);
        }

        public override SoundFX loadOGG(Byte[] audioData) {
            return new OGGMemorySoundFX(audioData);
        }

        public override SoundFX loadAudio(String fName) {
            try {
                return new FileSoundFX(fName);
            } catch (Exception e) {
                throw new UnsupportedAudioException(I18NString.Lookup("Audio_AudioFileFactory_LoadFileFailed"), e);
            }
        }

        public override SoundFX loadAudio(FileInfo fInfo) {
            try {
                return new FileSoundFX(fInfo);
            } catch (Exception e) {
                throw new UnsupportedAudioException(I18NString.Lookup("Audio_AudioFileFactory_LoadFileFailed"), e);
            }
        }
    }
}
