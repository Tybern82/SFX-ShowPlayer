using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.DMX {
    /**
     * Defines a currently recorded scene in the system. 
     */
    public interface ILXScene {

        /**
         * Used to access the value for the device currently assigned to the given output. Universe and device must fall in the range 1-512.
         */
        byte this[UInt16 universe, UInt16 device] { get; set; }
    }
}
