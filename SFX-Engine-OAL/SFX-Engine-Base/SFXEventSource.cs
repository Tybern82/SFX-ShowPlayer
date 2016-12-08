using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.kintoshmalae.SFXEngine.Audio;
using com.kintoshmalae.SFXEngine.Events;

namespace com.kintoshmalae.SFXEngine {
    /**
     * Interface to the various events triggered during playback of a sound effect.
     */
    public interface SFXEventSource {

        /**
         * Event triggered when the track begins playing.
         */
        EventRegister<SoundFX> onPlay { get; }

        /**
         * Event triggered when the track stops playing (either by coming to the end of the data,
         * or by fading out to silence).
         */
        EventRegister<SoundFX> onStop { get; }

        /**
         * Event triggered each time a group of samples is read from the track. Note that this
         * event will NOT trigger for each individual sample, but rather for each requested 
         * block of samples, as determined by the playback device.
         */
        EventRegister<SoundFX> onSample { get; }

        /**
         * Event triggered when the track is paused - pausing will temporarily suspend the output
         * of actual audio data from the track, but will continue to generate silence to fill any
         * requested buffers to prevent the playback device from removing the track from output.
         */
        EventRegister<SoundFX> onPause { get; }

        /**
         * Event triggered when the track resumes playback after being paused.
         */
        EventRegister<SoundFX> onResume { get; }

        /**
         * Event triggered when the track is moved to a different position in the data (except as part
         * of the normal reading/output process).
         */
        EventRegister<SoundFX> onSeek { get; }

        /**
         * Event triggered when the track is reset to the initial position. Please note that resetting a
         * stream will frequently also trigger a seek event, but this is not required/guaranteed. It is 
         * merely as a result that the default method of the internal classes for triggering a reset is
         * to perform a seek to 0.
         */
        EventRegister<SoundFX> onReset { get; }
    }
}
