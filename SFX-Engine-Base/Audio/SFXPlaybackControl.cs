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
        Play, Pause, Stop, Reset
    }

    /**
     * Specify the interface to control the playback of an audio sample. This interface defines the basic mechanisms necessary to implement
     * control over the playing of the audio sample, including pausing, stopping, resetting and seeking, either forward (always available),
     * or to an arbritary position in the sample. This interface also defines the current and ending positions of the sample.
     */
    public interface ISFXPlaybackControl {

        /**
         * Current mode this effect is in.
         */
        PlaybackMode currentState { get; }

        /**
         * Current sample position in the track.
         */
        uint currentSample { get; }

        /**
         * Current time position in the track.
         */
        TimeSpan currentTime { get; }

        /**
         * Format of the current track.
         */
        AudioSampleFormat audioFormat { get; }


        /**
         * Total number of samples in the whole track.
         */
        uint sampleLength { get; }

        /**
         * Total length of the whole track.
         */
        TimeSpan length { get; }

        /**
         * Determine whether this track supports unlimited seeking (not just forward seeking).
         */
        bool canSeek { get; }

        /**
         * Seek forward the given number of samples.
         */
        bool seekForward(uint samples);

        /**
         * Seek forward the given length of time.
         */
        bool seekForward(TimeSpan length);

        /**
         * Seek to the given absolute position in the track. Only possible to seek backwards from the current
         * position if #canSeek is true.
         */    
        bool seekTo(uint samplePosition);

        /**
         * Seek to the given absolute time in the track. Only possible to seek backwards from the current
         * position if #canSeek is true.
         */
        bool seekTo(TimeSpan time);

        /**
         * Reset the track to the initial position. Only possible if #canSeek is true.
         */
        bool reset();

        /**
         * Pause the current playback, without stopping the track entirely. Returns false if the track was 
         * already paused.
         */
        bool pause();

        /**
         * Resume playback of a previously paused track. Returns false if the track was not paused.
         */
        bool resume();

        /**
         * Stop playback on the track. Returns false if the track was not actually playing.
         */
        bool stop();
    }
}
