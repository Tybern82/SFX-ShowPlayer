using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using NAudio.Vorbis;

namespace com.kintoshmalae.SFXEngine.NAudio.Sources {
    class OGGMemorySoundFX : MemorySoundFX {

        private byte[] oggData;

        public OGGMemorySoundFX(VorbisWaveReader reader) : base(reader) { }

        public OGGMemorySoundFX(byte[] oggData) : this(new VorbisWaveReader(new MemoryStream(oggData))) {
            lock (_lock) {
                this.canDuplicate = true;
                this.oggData = oggData;
            }
        }

        protected override MemorySoundFX basicDup() {
            return (canDuplicate ? new OGGMemorySoundFX(oggData) : null);
        }
    }
}