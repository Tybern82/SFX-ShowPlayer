using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.DMX {
    public sealed class LXUtilities {
        private LXUtilities() { }

        /**
         * Used by the LXEngine to specify the default universe when the device is created. Since most uses are expected to be with
         * only a single device, this will default to the base address for the universe as 1.
         */
        public const UInt16 DefaultPrimaryUniverse = 1;

        /**
         * Used by the LXEngine to specify that the user is requesting a scene composed of all current universes operated by the engine.
         */
        public const UInt16 AllUniversesScene = 0;
    }
}
