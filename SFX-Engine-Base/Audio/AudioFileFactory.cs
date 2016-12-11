using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Exceptions;
using com.kintoshmalae.SFXEngine.I18N;

namespace com.kintoshmalae.SFXEngine.Audio {
    /**
     * Defines the audio types supported for in-memory reading. It is intended that where an audio library does not support
     * these types, an internal converter will (eventually) be implemented to convert the audio data into a format the 
     * library will support (most likely converted to IEEE-32bit-FloatingPoint used by NAudio for output, or an equivalent
     * for OpenAL). 'RAW' is defined as pure audio data, using the internal format used by the library of float data samples.
     */
    public enum AudioDataType {
        AAC, AIFF, AU, FLAC, M4A, MP3, MIDI, OGG, WAVE, WMA, WEBM, RAW /* , RA */
    }

    /**
     * Interface to the factory used to construct SoundFX instances for reading audio file data from an input file/memory.
     */
    public class AudioFileFactory {
        protected static readonly string LoadAudioFailedTypeMsg = "Audio_AudioFileFactory_LoadAudioFailedType";

        protected AudioFileFactory(IEnumerable<AudioDataType> supportedTypes, IEnumerable<AudioDataType> supportedMemoryTypes) {
            this.supportedTypes.AddRange(supportedTypes);
            this.supportedMemoryTypes.AddRange(supportedMemoryTypes);
        }

        /**
         * Determine the list of supported types capable of being read from a file by this library.
         */
        public List<AudioDataType> supportedTypes { get; } = new List<AudioDataType>();

        /**
         * Determine the list of supported types capable of being read from a memory buffer by this library. Note that this
         * list of types will generally be a subset (or the same as) the list of supported types readable from file.
         */
        public List<AudioDataType> supportedMemoryTypes { get; } = new List<AudioDataType>();

        /**
         * Load a SoundFX for reading audio data from the given file, based on the given type.
         */
        public SoundFX loadAudio(byte[] audioData, AudioDataType type) {
            if (!supportedMemoryTypes.Contains(type)) throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg));
            switch (type) {
                case AudioDataType.AAC:     return loadAAC(audioData);
                case AudioDataType.AIFF:    return loadAIFF(audioData);
                case AudioDataType.AU:      return loadAU(audioData);
                case AudioDataType.FLAC:    return loadFLAC(audioData);
                case AudioDataType.M4A:     return loadM4A(audioData);
                case AudioDataType.MIDI:    return loadMIDI(audioData);
                case AudioDataType.MP3:     return loadMP3(audioData);
                case AudioDataType.OGG:     return loadOGG(audioData);
                case AudioDataType.RAW:     return loadRAW(audioData);
                case AudioDataType.WAVE:    return loadWAVE(audioData);
                case AudioDataType.WEBM:    return loadWEBM(audioData);
                case AudioDataType.WMA:     return loadWMA(audioData);
                default:                    throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg));
            }
        }

        public virtual SoundFX loadAAC(byte[] audioData) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }
        public virtual SoundFX loadAIFF(byte[] audioData) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }
        public virtual SoundFX loadAU(byte[] audioData) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }
        public virtual SoundFX loadFLAC(byte[] audioData) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }
        public virtual SoundFX loadM4A(byte[] audioData) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }
        public virtual SoundFX loadMIDI(byte[] audioData) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }
        public virtual SoundFX loadMP3(byte[] audioData) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }
        public virtual SoundFX loadOGG(byte[] audioData) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }
        public virtual SoundFX loadWAVE(byte[] audioData) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }
        public virtual SoundFX loadWMA(byte[] audioData) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }
        public virtual SoundFX loadWEBM(byte[] audioData) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }
        public virtual SoundFX loadRAW(byte[] audioData) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }

        /**
         * Load a SoundFX from the given filename.
         */
        public virtual SoundFX loadAudio(string fName) { throw new UnsupportedAudioException(I18NString.Lookup(LoadAudioFailedTypeMsg)); }

        /**
         * Load a SoundFX from the given file reference.
         */
        public SoundFX loadAudio(FileInfo fInfo) {
            return loadAudio(fInfo.FullName);
        }
    }
}
