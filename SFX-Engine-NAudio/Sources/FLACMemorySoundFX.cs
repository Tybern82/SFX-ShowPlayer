using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NAudio.Flac;

namespace com.kintoshmalae.SFXEngine.NAudio.Sources {
    class FLACMemorySoundFX : MemorySoundFX {

        private byte[] flacData;

        public FLACMemorySoundFX(FlacReader reader) : base (reader) { }

        public FLACMemorySoundFX(byte[] flacData) : this (new FlacReader(new MemoryStream(flacData))) {
            lock (_lock) {
                this.canDuplicate = true;
                this.flacData = flacData;
            }
        }

        protected override MemorySoundFX basicDup() {
            return (canDuplicate ? new FLACMemorySoundFX(flacData) : null);
        }
    }
}