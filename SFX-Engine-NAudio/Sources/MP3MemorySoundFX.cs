using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Audio;
using com.kintoshmalae.SFXEngine.Events;
using NAudio.Wave;

namespace com.kintoshmalae.SFXEngine.NAudio.Sources {
    class MP3MemorySoundFX : MemorySoundFX {

        private byte[] mp3Data;

        public MP3MemorySoundFX(Mp3FileReader reader) : base(reader) {}

        public MP3MemorySoundFX(byte[] mp3Data) : this(new Mp3FileReader(new MemoryStream(mp3Data))) {
            lock (_lock) {
                this.canDuplicate = true;
                this.mp3Data = mp3Data;
            }
        }

        protected override MemorySoundFX basicDup() {
            return (canDuplicate ? new MP3MemorySoundFX(mp3Data) : null);
        }
    }
}
