using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using NAudio.Vorbis;
using com.kintoshmalae.SFXEngine.Audio;

namespace com.kintoshmalae.SFXEngine.NAudio.Sources {
    class OGGMemorySoundFX : WaveStreamSoundFX {

        private byte[] oggData;

        public OGGMemorySoundFX(VorbisWaveReader reader) : base(reader) { }

        public OGGMemorySoundFX(byte[] oggData) : this(new VorbisWaveReader(new MemoryStream(oggData))) {
            lock (_lock) {
                this.canDuplicate = true;
                this.oggData = oggData;
            }
        }

        protected override SoundFX basicDup {
            get {
                return (canDuplicate ? new OGGMemorySoundFX(oggData) : null);
            }
        }
    }
}