using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace com.kintoshmalae.SFXEngine.Audio {

    public delegate IAudioPlaybackEngine LoadDriverInstance(string devName);
    public delegate IAudioPlaybackEngine LoadDriverWithSample(string devName, AudioSampleFormat audioFormat);

    /**
     * Contains the structures used to load the correct audio library for the current OS and access the basic audio
     * systems from that library.
     */
    public sealed class AudioPlayback {

        private AudioPlayback() {}

        /**
         * Device controls access to the playback device implemented by the library.
         */
        private static IAudioPlaybackEngine _LoadedDevice = null;
        public static IAudioPlaybackEngine Device {
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

        /**
         * Array of possible driver names retrieved from the current library.
         */
        private static ReadOnlyCollection<string> _DriverNames = null;
        public static ReadOnlyCollection<string> DriverNames {
            get {
                if (_DriverNames == null) loadOSLibrary();
                return _DriverNames;
            }
        }

        private static LoadDriverInstance _loadDriverByName = null;
        private static LoadDriverWithSample _loadDriverWithSample = null;

        public static IAudioPlaybackEngine loadDriver(string name) {
            if (_loadDriverByName == null) loadOSLibrary();
            return _loadDriverByName(name);
        }

        public static IAudioPlaybackEngine loadDevice(string name, AudioSampleFormat audioFormat) {
            if (_loadDriverWithSample == null) loadOSLibrary();
            return _loadDriverWithSample(name, audioFormat);
        }

        /**
         * Supressing CA2001 since this method is used to actually load the OS-specific library to perform the actual work of the audio.
         */
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom")]
        private static void loadOSLibrary() {
            OperatingSystem os = Environment.OSVersion;
            Assembly asm;
            string libName;
            if ((os.Platform == PlatformID.Unix) || (os.Platform == PlatformID.MacOSX)) {
                // need to load the OpenAL devices
                asm = Assembly.LoadFrom("SFX-Engine-OAL.dll");
                libName = "OpenAL";
            } else {
                // need to load the NAudio devices
                asm = Assembly.LoadFrom("SFX-Engine-NAudio.dll");
                libName = "NAudio";
            }
            Type type = asm.GetType("com.kintoshmalae.SFXEngine." + libName + ".AudioFileFactory");
            _FileFactory = Activator.CreateInstance(type) as AudioFileFactory;

            type = asm.GetType("com.kintoshmalae.SFXEngine." + libName + ".AudioPlaybackEngine");
            _LoadedDevice = Activator.CreateInstance(type) as IAudioPlaybackEngine;

            _DriverNames = (type.GetMethod("driverNames").Invoke(null, null)) as ReadOnlyCollection<string>;
            _loadDriverByName = Delegate.CreateDelegate(typeof(LoadDriverInstance), type.GetMethod("loadDevice", new Type[] { typeof(string) })) as LoadDriverInstance;
            _loadDriverWithSample = Delegate.CreateDelegate(typeof(LoadDriverWithSample), type.GetMethod("loadDevice", new Type[] { typeof(string), typeof(AudioSampleFormat) })) as LoadDriverWithSample;
        }
    }
}
