using System;
using System.Runtime.Serialization;

namespace com.kintoshmalae.SFXEngine.Exceptions {

    /**
     * Base class for exceptions thrown by this library.
     */
     [Serializable]
    public class UnsupportedAudioException : Exception {

        public UnsupportedAudioException() : base() {}
        public UnsupportedAudioException(string message) : base(message) {}
        public UnsupportedAudioException(string message, Exception inner) : base(message, inner) {}
        protected UnsupportedAudioException(SerializationInfo info, StreamingContext ctx) : base(info, ctx) {}
    }
}
