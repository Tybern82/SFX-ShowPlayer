using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.Audio {

    /**
     * Interface to the basic playback engine for the audio system. This interface is implemented by each implementing library
     * to actually create the audio playback.
     */
    public interface AudioPlaybackEngine {

        AudioSampleFormat audioFormat { get; }
        
        void stop();
        void stop(SoundFX fx);

        void play(SoundFX fx);
    }
}
