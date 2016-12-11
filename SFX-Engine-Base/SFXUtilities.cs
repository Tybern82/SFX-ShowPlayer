using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Audio;
using com.kintoshmalae.SFXEngine.Events;

namespace com.kintoshmalae.SFXEngine {
    /**
     * Records a secondary EventRegister, used to cascade the triggers made against a base register, to trigger
     * against the secondary register.
     */
    class CascadeCallback<T> {
        private readonly EventRegister<T> callback;

        public CascadeCallback(EventRegister<T> c) {
            this.callback = c;
        }

        public void doCallback(T source, EventBaseArgs<T> args) {
            callback.triggerEvent(source, args);
        }
    }

    /**
     * Contains static utility methods used throughout the SFXEngine system.
     */
    public class SFXUtilities {
        /**
         * Helper method to attach the derived trigger to be activated each time the base trigger is activated.
         */
        public static void cascadeEvent<T>(EventRegister<T> baseTrigger, EventRegister<T> derivedTrigger) {
            baseTrigger.onTrigger += (new CascadeCallback<T>(derivedTrigger)).doCallback;
        }

        /**
         * Helper method to attach all the triggers in the derived event to be activated when the corresponding
         * trigger on the base event is activated.
         */
        public static void cascadeEvent(SFXEventSource baseEvents, SFXEventSource derivedEvents) {
            cascadeEvent<SoundFX>(baseEvents.onPlay, derivedEvents.onPlay);
            cascadeEvent<SoundFX>(baseEvents.onStop, derivedEvents.onStop);
            cascadeEvent<SoundFX>(baseEvents.onSample, derivedEvents.onSample);

            cascadeEvent<SoundFX>(baseEvents.onPause, derivedEvents.onPause);
            cascadeEvent<SoundFX>(baseEvents.onResume, derivedEvents.onResume);

            cascadeEvent<SoundFX>(baseEvents.onSeek, derivedEvents.onSeek);
            cascadeEvent<SoundFX>(baseEvents.onReset, derivedEvents.onReset);
        }
    }
}
