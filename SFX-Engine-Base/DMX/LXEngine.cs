using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.DMX {
    /**
     * Interface to the LX playback system for DMX control operations. Implemented by a DMX device module to support a particular type of 
     * device.
     */
    public interface ILXEngine {

        /**
         * Used to access the value for the device currently assigned to the given output. Universe and device must fall in the range 1-512.
         */
        byte this [UInt16 universe, UInt16 device] { get; set; }

        /**
         * Used to retrieve the scene associated with the given universe, or a scene comprising all current universes for a request for
         * universe '0'. Universe must be in the range 0-512.
         */
        ILXScene this[UInt16 universe] { get; }

        /**
         * Defines the primary universe used by this device. It is expected that most LXEngine implementations will be used with a single
         * device in most situations. Defaults to LXUtilities.DefaultPrimaryUniverse (1).
         */
        UInt16 primaryUniverse { get; }

        /**
         * Retrieve a collection of all the universes currently supported on this engine.
         */
        ReadOnlyCollection<UInt16> universes { get; }

        /**
         * Helper method to immediately trigger a blackout effect on the current device.
         */
        void blackout();

        /**
         * Helper method to load an entire scene immediately. Can be combined with #blackout() to restore the previous state following the
         * blackout. Note that there is NO FADE on this scene - the settings will be applied immediately (or as fast as processing supports).
         * The bool parameter determines what happens with addresses which have current values, but are not included in the scene. By default,
         * these are just ignored - if the parameter is true, these values will be set to '0' instead.
         */
        void loadScene(ILXScene scene, bool clearUnset = false);
    }
}
