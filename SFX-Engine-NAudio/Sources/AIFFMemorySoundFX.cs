using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Audio;
using NAudio.Wave;

namespace com.kintoshmalae.SFXEngine.NAudio.Sources {
    class AIFFMemorySoundFX : WaveStreamSoundFX {

        private byte[] aiffData;

        public AIFFMemorySoundFX(AiffFileReader reader) : base(reader) {}

        public AIFFMemorySoundFX(byte[] aiffData) : this(new AiffFileReader(new MemoryStream(aiffData))) {
            lock (_lock) {
                this.canDuplicate = true;
                this.aiffData = aiffData;
            }
        }

        protected override SoundFX basicDup {
            get {
                return (canDuplicate ? new AIFFMemorySoundFX(aiffData) : null);
            }
        }
    }
}
