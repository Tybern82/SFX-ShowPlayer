using System;

namespace com.kintoshmalae.SFXEngine.Exceptions {

    /**
     * Base class for exceptions thrown by this library.
     */
    public class UnsupportedAudioException : Exception {

        public UnsupportedAudioException() : base() {}
        public UnsupportedAudioException(string message) : base(message) {}
        public UnsupportedAudioException(string message, Exception inner) : base(message, inner) {}
    }
}
