using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Events;

namespace com.kintoshmalae.SFXEngine.Audio {
    /**
     * Determine the current state of the fade in the track.
     */
    public enum FadeState {
        Silence,
        FadeIn,
        FullVolume,
        FadeOut
    }

    /**
     * Interface to a Fadeable sound FX source. Provides the structure of methods necessary to manipulate the volume, and control
     * fading in and out of the source.
     */
    public interface SFXFadeControl {

        /**
         * The base volume applied to the effect. This volume modifies the original source and is used as the basis of the fade methods.
         * This value acts as a percentage multiplier to the audio samples.
         */
        uint volume { get; set; }

        /**
         * Determine whether this source will automatically fade out after a preset length of time (as specified in AutoFadeOutAt). This
         * parameter exists to allow the fade-out to be triggered immediately on the start of the track.
         */
        bool hasAutoFade { get; set; }

        /**
         * Initial seek position on a reset.
         */
        TimeSpan initialSeekTo { get; set; }

        /**
         * Duration of the fade-in over the beginning of this track. May be TimeSpan.Zero to specify that the source should start at
         * full volume (as modified by Volume).
         */
        TimeSpan fadeInDuration { get; set; }
        /**
         * Duration of the fade-out at the end of the track. May be TimeSpan.Zero to specify that the track will immediately terminate when
         * asked to fade-out. Equivalent of FadeInDuration for the end of the track.
         */
        TimeSpan fadeOutDuration { get; set; }
        /**
         * Point in the track at which to automatically trigger a fade-out. This parameter will only be applied if 'hasAutoFade' is also 
         * set to true, otherwise this parameter should be silently ignored.
         */
        TimeSpan autoFadeOutAt { get; set; }

        /**
         * Total playable length of the track given any current fade controls.
         */
        TimeSpan playableLength { get; }

        /**
         * Current fade state.
         */
         FadeState fadeState { get; }

        /**
         * Event triggered when the fade-in is completed. Beginning of fade-in will trigger the onPlay event.
         */
         EventRegister<SoundFX> onFadeIn { get; }

        /**
         * Event triggered when the fade-out begins. End of fade-out will trigger the onStop event.
         */
         EventRegister<SoundFX> onFadeOut { get; }


        /**
         * Trigger the start of a fade-in of this source.
         */
        void beginFadeIn();

        /**
         * Trigger the start of the fade-out of this source. When the source has completely faded, the output will terminate.
         */
        void beginFadeOut();
    }
}
