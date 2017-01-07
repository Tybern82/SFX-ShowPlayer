using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Events;
using com.kintoshmalae.SFXEngine.I18N;
using System.Threading;

namespace com.kintoshmalae.SFXEngine.Audio.Sources {
    public class BufferedSoundFX : SoundFX {

        public uint BufferSize { get; protected set; }

        private float[] activeBuffer;
        private float[] secondaryBuffer;
        private object bufferLock = new object();

        private SoundFX source;

        public BufferedSoundFX(SoundFX source) {
            this.source = source;
            this.canDuplicate = source.canDuplicate;
            this.canSeek = source.canSeek;
            this.audioFormat = source.audioFormat;
            this.length = source.length;
            this.initialSeekTo = source.currentTime;
            this.BufferSize = 10 * audioFormat.samplesPerSecond;
            ThreadPool.QueueUserWorkItem(buffer, this); // schedule a secondary buffer to be filled
        }

        protected override SoundFX basicDup {
            get {
                return new BufferedSoundFX(source.dup());
            }
        }

        public override Boolean seekTo(UInt32 samplePosition) {
            lock (_lock) {
                lock (bufferLock) {
                    bool _result = source.seekTo(samplePosition);
                    if (_result) {
                        onSeek.triggerEvent(this, new SoundEventArgs(this, PlaybackEvent.Seek, samplePosition));
                        activeBuffer = null;
                        secondaryBuffer = null;
                        ThreadPool.QueueUserWorkItem(buffer, this);
                    }
                    return _result;
                }
            }
        }

        public override Boolean seekForward(UInt32 samples) {
            return seekTo(_currentSample + samples);
        }

        public override SoundFX cache() {
            lock (_lock) {
                // only load the buffer if it isn't already loaded, is empty, or has been exhausted already
                if ((activeBuffer == null) || (activeBuffer.Length == 0) || ((activeBuffer.Length - position) == 0)) loadMainBuffer();
            }
            return this;
        }

        private long position;

        protected override UInt32 readSample(Single[] buffer, UInt32 offset, UInt32 count) {
            lock (_lock) {
                // If the buffer is missing, or empty, try to load a new buffer
                if ((activeBuffer == null) || (activeBuffer.Length == 0)) loadMainBuffer();
                if ((activeBuffer == null) || (activeBuffer.Length == 0)) {
                    // if we've tried to load a new buffer, but still have no data to read, we must be at the end of the stream
                    return 0;
                }
                uint samplesCopied = 0;
                while (samplesCopied < count) {
                    // See how much data is currently in the buffer...
                    var availableSamples = activeBuffer.Length - position;
                    // if the buffer has been exhausted, try to load a new buffer
                    if (availableSamples == 0) {
                        loadMainBuffer();
                        availableSamples = activeBuffer.Length - position;
                    }
                    // we've tried to replace the buffer, but still have no data, we must be at the end of the stream, just return
                    // what we've managed to get so far
                    if (availableSamples == 0) return samplesCopied;
                    // Make sure we don't read more than the requested amount of data from the buffer, and not more than we have
                    // available in the current buffer
                    var samplesToCopy = Math.Min(availableSamples, count-samplesCopied);
                    // Copy from the active buffer into the result buffer
                    Array.Copy(activeBuffer, position, buffer, offset + samplesCopied, samplesToCopy);
                    // Update the current positions
                    position += samplesToCopy;
                    samplesCopied += (uint)samplesToCopy;
                }
                return samplesCopied;
            }
        }

        private static void buffer(object obj) {
            BufferedSoundFX snd = (obj as BufferedSoundFX);
            if (snd == null) return;
            lock (snd.bufferLock) {
                if ((snd.secondaryBuffer == null) || (snd.secondaryBuffer.Length == 0)) {
                    // only buffer if it doesn't already exist (this method may be scheduled multiple times from different points)
                    // In particular, this method is called by #loadMainBuffer to ensure that the secondary buffer is present before
                    // swapping out the active buffer - if the ThreadPool has already scheduled the read, this should already be 
                    // filled
                    var nBuffer = new List<float>();
                    var readBuffer = new float[snd.audioFormat.samplesPerSecond];
                    uint samplesRead;
                    long totalSamples = 0;
                    while ((samplesRead = snd.source.read(readBuffer, 0, (uint)readBuffer.Length)) > 0) {
                        nBuffer.AddRange(readBuffer.Take((int)samplesRead));
                        totalSamples += samplesRead;
                        if (totalSamples >= snd.BufferSize) break;
                    }
                    snd.secondaryBuffer = nBuffer.ToArray();
                }
            }
        }

        private void loadMainBuffer() {
            lock (_lock) {
                lock (bufferLock) {
                    // Ensure the secondary buffer is filled
                    buffer(this);
                    // Move the secondary buffer to the main buffer
                    activeBuffer = secondaryBuffer;
                    // and reset the secondary buffer to be empty
                    secondaryBuffer = null;
                    // Move to the start position in the new main buffer
                    position = 0;
                    // Start a thread to preload the secondary buffer
                    ThreadPool.QueueUserWorkItem(buffer, this);
                }
            }
        }
    }
}
