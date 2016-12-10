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
        AAC, AIFF, FLAC, MP3, WAVE, OGG, WMA, WEBM, RAW
    }

    public interface AudioFileFactory {

        List<AudioDataType> supportedTypes { get; }

        SoundFX loadAudio(byte[] audioData, AudioDataType type);
        SoundFX loadAudio(string fName);
        SoundFX loadAudio(FileInfo fInfo);
    }
}
