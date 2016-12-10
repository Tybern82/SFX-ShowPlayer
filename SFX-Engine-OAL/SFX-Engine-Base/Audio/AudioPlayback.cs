using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace com.kintoshmalae.SFXEngine.Audio {
    /**
     * Contains the structures used to load the correct audio library for the current OS and access the basic audio
     * systems from that library.
     */
    public class AudioPlayback {
        /**
         * Device controls access to the playback device implemented by the library.
         */
        private static AudioPlaybackEngine _LoadedDevice = null;
        public static AudioPlaybackEngine Device {
            get {
                if (_LoadedDevice == null) loadOSLibrary();
                return _LoadedDevice;
            }
        }

        /**
         * Factory controls access to the factory used to construct audio file reader effects using the library.
         */
        private static AudioFileFactory _FileFactory = null;
        public static AudioFileFactory Factory {
            get {
                if (_FileFactory == null) loadOSLibrary();
                return _FileFactory;
            }
        }

        private static void loadOSLibrary() {
            OperatingSystem os = Environment.OSVersion;
            if ((os.Platform == PlatformID.Unix) || (os.Platform == PlatformID.MacOSX)) {
                // need to load the OpenAL devices
                Assembly asm = Assembly.LoadFrom("SFX-Engine-OAL.dll");

                Type type = asm.GetType("com.kintoshmalae.SFXEngine.OpenAL.AudioPlaybackEngine");
                _LoadedDevice = (AudioPlaybackEngine)Activator.CreateInstance(type);

                type = asm.GetType("com.kintoshmalae.SFXEngine.OpenAL.AudioFileFactory");
                _FileFactory = (AudioFileFactory)Activator.CreateInstance(type);
            } else {
                // need to load the NAudio devices
                Assembly asm = Assembly.LoadFrom("SFX-Engine-NAudio.dll");

                Type type = asm.GetType("com.kintoshmalae.SFXEngine.NAudio.AudioPlaybackEngine");
                _LoadedDevice = (AudioPlaybackEngine)Activator.CreateInstance(type);

                type = asm.GetType("com.kintoshmalae.SFXEngine.NAudio.AudioFileFactory");
                _FileFactory = (AudioFileFactory)Activator.CreateInstance(type);
            }
        }
    }
}
