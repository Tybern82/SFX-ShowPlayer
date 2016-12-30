using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using com.kintoshmalae.SFXEngine.Audio;
using NAudio.Wave;

namespace com.kintoshmalae.SFXEngine.NAudio.Sources {
    class FileSoundFX : WaveStreamSoundFX {

        private FileInfo fName;

        public FileSoundFX(string fname) : this (new FileInfo(fname)) {}

        public FileSoundFX(FileInfo fname) : base(new AudioFileReader(fname.FullName)) {
            lock (_lock) {
                this.fName = fname;
                this.canDuplicate = true;
            }
        }

        protected override SoundFX basicDup {
            get {
                return (fName.Exists ? new FileSoundFX(fName) : null);
            }
        }
    }
}
