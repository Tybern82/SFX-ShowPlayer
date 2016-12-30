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

        public void doCallback(object source, EventBaseArgs<T> args) {
            callback.triggerEvent(source, args);
        }
    }

    /**
     * Contains static utility methods used throughout the SFXEngine system.
     */
    public sealed class SFXUtilities {
        private SFXUtilities() {}

        /**
         * Helper method to attach the derived trigger to be activated each time the base trigger is activated.
         */
        public static void cascadeEvent<T>(EventRegister<T> baseTrigger, EventRegister<T> derivedTrigger) {
            if ((baseTrigger == null) || (derivedTrigger == null)) return;
            baseTrigger.onTrigger += (new CascadeCallback<T>(derivedTrigger)).doCallback;
        }

        /**
         * Helper method to attach all the triggers in the derived event to be activated when the corresponding
         * trigger on the base event is activated.
         */
        public static void cascadeEvent(ISFXEventSource baseEvents, ISFXEventSource derivedEvents) {
            if ((baseEvents == null) || (derivedEvents == null)) return;

            cascadeEvent<SoundFX>(baseEvents.onPlay, derivedEvents.onPlay);
            cascadeEvent<SoundFX>(baseEvents.onStop, derivedEvents.onStop);
            cascadeEvent<SoundFX>(baseEvents.onSample, derivedEvents.onSample);

            cascadeEvent<SoundFX>(baseEvents.onPause, derivedEvents.onPause);
            cascadeEvent<SoundFX>(baseEvents.onResume, derivedEvents.onResume);

            cascadeEvent<SoundFX>(baseEvents.onSeek, derivedEvents.onSeek);
            cascadeEvent<SoundFX>(baseEvents.onReset, derivedEvents.onReset);
        }

        /**
         * Used by the LXEngine to specify the default universe when the device is created. Since most uses are expected to be with
         * only a single device, this will default to the base address for the universe as 1.
         */
        public static readonly UInt16 DefaultPrimaryUniverse = 1;

        /**
         * Used by the LXEngine to specify that the user is requesting a scene composed of all current universes operated by the engine.
         */
        public static readonly UInt16 AllUniversesScene = 0;
    }
}
