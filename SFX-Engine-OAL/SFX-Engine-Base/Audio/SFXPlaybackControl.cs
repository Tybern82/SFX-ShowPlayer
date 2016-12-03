using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.Audio {
    /**
     * Specify the current mode for the audio sample:
     *      Play    - default mode, indicates standard playback mode
     *      Pause   - audio sample is temporarily stopped, sample will continue to generate silence until resumed/stopped
     *      Stop    - audio sample has completed, no more audio data will be issued by this sample (unless reset)
     */
    public enum PlaybackMode {
        Play, Pause, Stop
    }

    /**
     * Specify the interface to control the playback of an audio sample. This interface defines the basic mechanisms necessary to implement
     * control over the playing of the audio sample, including pausing, stopping, resetting and seeking, either forward (always available),
     * or to an arbritary position in the sample. This interface also defines the current and ending positions of the sample.
     */
    public interface SFXPlaybackControl {

        PlaybackMode currentState { get; }
        uint currentSample { get; }
        TimeSpan currentTime { get; }

        uint sampleLength { get; }
        TimeSpan length { get; }

        bool canSeek { get; }

        bool seekForward(uint samples);
        bool seekForward(TimeSpan length);

        bool seekTo(uint sample);
        bool seekTo(TimeSpan time);

        bool reset();
    }
}
