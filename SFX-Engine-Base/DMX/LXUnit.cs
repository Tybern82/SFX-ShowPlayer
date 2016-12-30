using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.DMX {
    public enum ChannelType {
        // Basic intensity of the light - acts as a master brightness on the unit
        Intensity,
        // Color selection - used to define channels which act as color sources
        Red, Green, Blue, White, ColorSelect,
        // Effects on the light - strobing, patterns, etc
        Strobe, GoboSelect,
        // Position of movable units - Recommended to specify X as plane aligned with the base of the light, Y as plane cutting light in half, Z as plane cutting head/bulb across
        // ie X - rotation of the arm, Y - angle of the arm, Z - rotation of the head/bulb on the arm
        XPosition, YPosition, ZPosition,
        // Defines any unspecified DMX control channel which doesn't fit into any of the given categories.
        Generic
    }

    /**
     * Defines a logical LX device comprising a single lighting unit (ie one LED light, one smoke machine, etc). Use of these devices, rather
     * than pure DMX allows more structured assignment, as well as more flexibility to control groups of units and adjust overall intensities
     * more easily.
     */
    public interface ILXUnit {
        /**
         * The name for this particular type of LX unit. Used to display to the user so they know what device they are looking at.
         */
        string typeName { get; }

        /**
         * Defines the base address where this device is currently located.
         */
        UInt16 baseAddress { get; set; }

        /**
         * Defines the number of consecutive addresses assigned to this unit. This unit will occupy the addresses from [baseAddress+0..baseAddress+addressRange-1]
         */
        UInt16 addressRange { get; }

        IChannel this[UInt16 address] { get; }
    }

    public interface IChannel {
        ChannelType type { get; }
        byte value { get; set; }
    }
}
