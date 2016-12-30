using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Audio;
using NAudio.Wave;

namespace com.kintoshmalae.SFXEngine.NAudio.Sources {
    class WAVEMemorySoundFX : WaveStreamSoundFX {

        private byte[] waveData;

        public WAVEMemorySoundFX(WaveFileReader reader) : base(reader) { }

        public WAVEMemorySoundFX(byte[] waveData) : this (new WaveFileReader(new MemoryStream(waveData))) {
            lock (_lock) {
                this.canDuplicate = true;
                this.waveData = waveData;
            }
        }

        protected override SoundFX basicDup {
            get {
                return (canDuplicate ? new WAVEMemorySoundFX(waveData) : null);
            }
        }
    }
}