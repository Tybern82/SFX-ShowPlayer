using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Events;
using com.kintoshmalae.SFXEngine.I18N;

namespace com.kintoshmalae.SFXEngine.Audio {

    /**
     * Basic sound effect base class - acts as both an event source, and provides playback control. This class provides most of 
     * the required structures for these interfaces, with the implementing classes for various forms of effect having to override
     * only a few methods: ReadSample (for retrieving the actual audio data from the track), seekTo (when seeking is possible), 
     * length/sampleLength (for determining the actual length of the audio data). Other methods may be overriden where the 
     * underlying data provides a more efficient means of implementing (ie override seekForward where the data is entirely cached
     * in memory to just update the current read position).
     */
    public abstract class SoundFX : SFXEventSource, SFXPlaybackControl, SFXFadeControl {
        #region Static Constants
        public static readonly uint FullVolume = 100;
        public static readonly uint HalfVolume = 50;
        public static readonly uint NoVolume = 0;
        #endregion

        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SoundFX));
        protected object _lock = new object();

        #region SFXEventSource
        public EventRegister<SoundFX> onPause { get; } = new EventRegister<SoundFX>();
        public EventRegister<SoundFX> onPlay { get; } = new EventRegister<SoundFX>(); 
        public EventRegister<SoundFX> onReset { get; } = new EventRegister<SoundFX>();
        public EventRegister<SoundFX> onResume { get; } = new EventRegister<SoundFX>();
        public EventRegister<SoundFX> onSample { get; } = new EventRegister<SoundFX>();
        public EventRegister<SoundFX> onSeek { get; } = new EventRegister<SoundFX>();
        public EventRegister<SoundFX> onStop { get; } = new EventRegister<SoundFX>();
        #endregion

        #region SFXFadeControl
        public EventRegister<SoundFX> onFadeIn { get; } = new EventRegister<SoundFX>();
        public EventRegister<SoundFX> onFadeOut { get; } = new EventRegister<SoundFX>();

        public FadeState fadeState { get; private set; } = FadeState.FullVolume;
        public uint volume { get; set; } = FullVolume;
        public bool hasAutoFade { get; set; } = false;
        public TimeSpan initialSeekTo { get; set; } = TimeSpan.Zero;
        public TimeSpan autoFadeOutAt { get; set; } = TimeSpan.Zero;
        public TimeSpan fadeInDuration { get; set; } = TimeSpan.Zero;
        public TimeSpan fadeOutDuration { get; set; } = TimeSpan.Zero;

        public TimeSpan playableLength {
            get {
                lock (_lock) {
                    return ((hasAutoFade ? autoFadeOutAt + fadeOutDuration : length) - currentTime);
                }
            }
        }

        public void beginFadeIn() {
            logger.Debug("Begin Fade-In [" + this + "]");
            lock (_lock) {
                fadePosition = 0;
                fadeDuration = (long)Math.Round(fadeInDuration.TotalSeconds * audioFormat.sampleRate, MidpointRounding.AwayFromZero);
                fadeState = FadeState.FadeIn;
            }
        }

        public void beginFadeOut() {
            logger.Debug("Begin Fade-Out [" + this + "]");
            lock (_lock) {
                fadePosition = 0;
                fadeDuration = (long)Math.Round(fadeOutDuration.TotalSeconds * audioFormat.sampleRate, MidpointRounding.AwayFromZero);
                fadeState = FadeState.FadeOut;
            }
        }

        public void dupFadeState(SFXFadeControl fade) {
            lock (_lock) {
                this.fadeState = fade.fadeState;
                this.volume = fade.volume;
                this.hasAutoFade = fade.hasAutoFade;
                this.initialSeekTo = fade.initialSeekTo;
                this.autoFadeOutAt = fade.autoFadeOutAt;
                this.fadeInDuration = fade.fadeInDuration;
                this.fadeOutDuration = fade.fadeOutDuration;
                seekToFade();
            }
        }

        /**
         * Helper method to adjust the internal fade positions used to control the fade-in/out sequence to match a new
         * position in the track (or manipulation of the fade parameters).
         */
        protected void seekToFade() {
            lock (_lock) {
                switch (fadeState) {
                    case FadeState.FadeIn:
                    case FadeState.FadeOut:
                        fadeDuration = (long)Math.Round(fadeInDuration.TotalSeconds * audioFormat.sampleRate, MidpointRounding.AwayFromZero);
                        break;
                }

                switch (fadeState) {
                    case FadeState.FadeIn:
                        fadePosition = currentSample - (long)Math.Round(initialSeekTo.TotalSeconds * audioFormat.samplesPerSecond, MidpointRounding.ToEven);
                        break;

                    case FadeState.FadeOut:
                        fadePosition = currentSample - autoFadeOutSample;
                        break;

                }
            }
        }
        #endregion

        #region SFXPlaybackControl

        public PlaybackMode currentState { get; protected set; } = PlaybackMode.Reset;
        public TimeSpan length { get; protected set; } = TimeSpan.Zero;
        public AudioSampleFormat audioFormat { get; protected set; } = AudioSampleFormat.DefaultFormat;
        public bool canSeek { get; protected set; } = false;

        public uint sampleLength { get { return (uint)Math.Round(length.TotalSeconds * audioFormat.samplesPerSecond, MidpointRounding.ToEven); } }
        public virtual uint currentSample { get { return _currentSample; } }
        public virtual TimeSpan currentTime { get { return new TimeSpan((_currentSample * TimeSpan.TicksPerSecond) / audioFormat.samplesPerSecond); } }

        public virtual bool seekForward(uint samples) {
            lock (_lock) {
                if (canSeek) return seekTo(currentSample + samples);

                float[] buffer = new float[Math.Min(samples, audioFormat.samplesPerSecond)];
                long samplesRead = 0;
                uint currRead;
                while (samplesRead < samples) {
                    currRead = read(buffer, 0, (uint)Math.Min(buffer.Length, samples - samplesRead));
                    if (currRead == 0) return true; // reached the end of the input, can't seek any further forward
                    samplesRead += currRead;
                }
                onSeek.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Seek, _currentSample));
                seekToFade();   // adjust fade to new position
                return true;
            }
        }

        public virtual bool seekForward(TimeSpan ts) {
            uint samples = (uint)Math.Round(ts.TotalSeconds * audioFormat.samplesPerSecond, MidpointRounding.ToEven);
            return seekForward(samples);
        }

        public virtual bool seekTo(uint samplePosition) {
            lock (_lock) {
                if (samplePosition == _currentSample) {
                    onSeek.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Seek, _currentSample));
                    return true;
                }
                if (samplePosition > _currentSample) return seekForward(samplePosition - _currentSample);
                return false;   // by default we can't seek to an arbitrary position
            }
        }

        public virtual bool seekTo(TimeSpan ts) {
            uint samplePosition = (uint)Math.Round(ts.TotalSeconds * audioFormat.samplesPerSecond, MidpointRounding.ToEven);
            return seekTo(samplePosition);
        }

        public virtual bool reset() {
            lock (_lock) {
                bool _result = seekTo(initialSeekTo);
                if (_result) {
                    onReset.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Reset, _currentSample));
                    currentState = PlaybackMode.Reset;
                }
                return _result;
            }
        }

        public bool pause() {
            lock (_lock) {
                if (currentState != PlaybackMode.Play) return false;
                currentState = PlaybackMode.Pause;
                onPause.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Pause, _currentSample));
                return true;
            }
        }

        public bool resume() {
            lock(_lock) {
                if (currentState != PlaybackMode.Pause) return false;
                currentState = PlaybackMode.Play;   // couldn't pause unless we were playing
                onResume.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Resume, _currentSample));
                return true;
            }
        }

        public bool stop() {
            lock (_lock) {
                if ((currentState != PlaybackMode.Play) && (currentState != PlaybackMode.Pause)) {
                    // must be either Play or Pause to be able to Stop
                    if (currentState == PlaybackMode.Pause) resume();   // resume first, ensures those waiting on onResume complete properly
                    currentState = PlaybackMode.Stop;
                    onStop.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Stop, _currentSample));
                    return true;
                }
                return false;
            }
        }

        #endregion
        
        /**
         * Determine whether this effect can be successfully duplicated.
         */
        public bool canDuplicate { get; protected set; } = false;

        /**
         * Duplicate this effect. Override to support an effect which allows duplication (most effects should support this,
         * unless they are reading from a single-use stream, like a network socket). Method will throw an exception if 
         * duplication is not supported.
         */
        public virtual SoundFX dup() {
            throw new InvalidOperationException(I18NString.Lookup("Audio_SoundFX_DupFailed"));
        }

        public uint read(float[] buffer, uint offset, uint count) {
            lock (_lock) {
                // Check whether we are currently in the correct playback mode for standard operation, or whether we need special processing
                switch (currentState) {
                    case PlaybackMode.Pause:
                        // Currently paused - just return a buffer of silence
                        return readSilence(buffer, offset, count);

                    case PlaybackMode.Stop:
                        // Currently stopped - return no data
                        return 0;

                    case PlaybackMode.Reset:
                        // We have yet to start playback, trigger the initial playback event and make sure we are starting at the correct position
                        currentState = PlaybackMode.Play;
                        seekTo(initialSeekTo);
                        onPlay.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Play, _currentSample));
                        break;
                }
                // Retrieve a section of actual audio data...
                uint _result = readSample(buffer, offset, count);
                if (_result == 0) stop();   // we're out of audio data, stop the playback
                if (_result != 0) {         // ... otherwise, we need to do manipulate the volume and possible fade-out trigger
                    // Master Volume on the track
                    if (volume != FullVolume) {
                        float volumeMult = volume / 100f;
                        for (int x = 0; x < _result; x++) {
                            buffer[offset + x] *= volumeMult;
                        }
                    }
                    if (hasAutoFade) {
                        if ((fadeState == FadeState.FullVolume) && (_currentSample + _result > autoFadeOutSample)) {
                            // need to automatically trigger the fade-out
                            uint unfaded = (uint)(autoFadeOutSample - _currentSample);
                            beginFadeOut();
                            if (unfaded > 0) {
                                // not fading from the beginning of the buffer, need to do a partial fade-out, otherwise just wait for the full processing
                                doFadeOut(buffer, offset + unfaded, _result - unfaded);
                                // update the current details and trigger the sample event here since we can't continue into standard fade-processing.
                                _currentSample += _result;
                                onSample.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Sample, _currentSample));
                                return _result;
                            }
                        }
                    }
                    _currentSample += _result;
                    onSample.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Sample, _currentSample));

                    // Continue with a previously started fade operation
                    switch (fadeState) {
                        case FadeState.FadeIn:
                            doFadeIn(buffer, offset, _result);
                            break;

                        case FadeState.FadeOut:
                            doFadeOut(buffer, offset, _result);
                            break;

                        case FadeState.Silence:
                            readSilence(buffer, offset, count);
                            stop();
                            return 0;
                    }
                }
                return _result;
            }
        }

        /**
         * Internal method implemented by subclases to actually read the audio data into the buffer for manipulation.
         */
        protected abstract uint readSample(float[] buffer, uint offset, uint count);

        #region Internal Helpers
        protected uint _currentSample = 0;

        private long fadePosition = 0;
        private long fadeDuration = 0;

        private long autoFadeOutSample {
            get {
                return (long)Math.Round(autoFadeOutAt.TotalSeconds * audioFormat.samplesPerSecond, MidpointRounding.AwayFromZero);
            }
        }

        protected uint readSilence(float[] buffer, uint offset, uint count) {
            Array.Clear(buffer, (int)offset, (int)count);
            return count;
        }

        /**
         * Helper method to perform the fade-out of the current sample buffer. The buffer should contain
         * complete samples (with all the channels for a single sample point present in the same buffer,
         * not with partial channels in one buffer, then part in the next). An error WILL occur if only
         * part of the channels for a single sample point are included in the buffer.
         */
        private void doFadeOut(float[] buffer, uint offset, uint count) {
            for (uint currSample = 0; currSample < count; currSample++) { 
                // calculate the relative volume control
                float mult = 1.0f - (fadePosition / (float)fadeDuration);
                for (int ch = 0; ch < audioFormat.channelCount; ch++) {
                    // adjust volume for all the channels in this sample
                    buffer[offset + currSample] *= mult;
                }
                fadePosition++;
                if (fadePosition > fadeDuration) {
                    fadeState = FadeState.Silence;
                    // clear the end of the buffer and end
                    readSilence(buffer, currSample + offset, count - currSample);
                    break;
                }
            }
        }

        /**
         * Helper method to perform the fade-in of the current sample buffer. See #doFadeOut for restrictions
         * on the structure of the buffer which also apply to this method.
         */
        private void doFadeIn(float[] buffer, uint offset, uint count) {
            for (uint currSample = 0; currSample < count; currSample++) { 
                // calculate the relative volume control
                float mult = (fadePosition / (float)fadeDuration);
                for (int ch = 0; ch < audioFormat.channelCount; ch++) {
                    buffer[offset + currSample] *= mult;
                }
                fadePosition++;
                if (fadePosition > fadeDuration) {
                    fadeState = FadeState.FullVolume;
                    // no need to adjust any more samples, we are now at full volume
                    break;
                }
            }
        }
        #endregion
    }
}
