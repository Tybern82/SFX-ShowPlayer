using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
    public interface AudioFileFactory {

        /**
         * Determine the list of supported types capable of being read from a file by this library.
         */
        List<AudioDataType> supportedTypes { get; }

        /**
         * Determine the list of supported types capable of being read from a memory buffer by this library. Note that this
         * list of types will generally be a subset (or the same as) the list of supported types readable from file.
         */
        List<AudioDataType> supportedMemoryTypes { get; }

        /**
         * Load a SoundFX for reading audio data from the given file, based on the given type.
         */
        SoundFX loadAudio(byte[] audioData, AudioDataType type);

        /**
         * Load a SoundFX from the given filename.
         */
        SoundFX loadAudio(string fName);

        /**
         * Load a SoundFX from the given file reference.
         */
        SoundFX loadAudio(FileInfo fInfo);
    }
}
