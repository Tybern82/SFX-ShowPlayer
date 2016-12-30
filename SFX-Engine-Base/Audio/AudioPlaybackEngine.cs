using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.Audio {

    /**
     * Interface to the basic playback engine for the audio system. This interface is implemented by each implementing library
     * to actually create the audio playback.
     */
    public interface IAudioPlaybackEngine {

        AudioSampleFormat audioFormat { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop")]
        void stop();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop")]
        void stop(SoundFX fx);

        void play(SoundFX fx);

        /**
         * Required static methods.
         * 
         * Implementing classes MUST provide static methods of the following signatures to allow the AudioPlayback class to correctly
         * access non-default instances.
         */
        // static AudioPlaybackEngine loadDevice(string);
        // static AudioPlaybackEngine loadDevice(string, AudioSampleFormat);
        // static ReadOnlyCollection<string> driverNames();
    }
}
