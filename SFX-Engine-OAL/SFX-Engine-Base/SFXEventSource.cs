using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.kintoshmalae.SFXEngine.Audio;
using com.kintoshmalae.SFXEngine.Events;

namespace com.kintoshmalae.SFXEngine {

    public interface SFXEventSource {

        EventRegister<SoundFX> onPlay { get; }
        EventRegister<SoundFX> onStop { get; }
        EventRegister<SoundFX> onSample { get; }

        EventRegister<SoundFX> onPause { get; }
        EventRegister<SoundFX> onResume { get; }

        EventRegister<SoundFX> onSeek { get; }
        EventRegister<SoundFX> onReset { get; }
    }
}
