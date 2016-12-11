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

        private List<AudioDataType> _supportedMemoryTypes = new List<AudioDataType>(new []{AudioDataType.MP3});
        public List<AudioDataType> supportedMemoryTypes {
            get {
                return _supportedMemoryTypes;
            }
        }

        private List<AudioDataType> _supportedTypes = new List<AudioDataType>(new [] {AudioDataType.MP3, AudioDataType.WAVE});
        public List<AudioDataType> supportedTypes {
            get {
                return _supportedTypes;
            }
        }

        public SoundFX loadAudio(FileInfo fInfo) {
            throw new NotImplementedException();
        }

        public SoundFX loadAudio(String fName) {
            throw new NotImplementedException();
        }

        public SoundFX loadAudio(Byte[] audioData, AudioDataType type) {
            switch (type) {
                case AudioDataType.MP3:
                    return new MP3MemorySoundFX(audioData);

                default:
                    throw new UnsupportedAudioException(I18NString.Lookup("Audio_AudioFileFactory_LoadAudioFailedType"));
            }
        }
    }
}
