using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.Events {
    public class EventBaseArgs <T> {
        public T Source { get; private set; }

        public EventBaseArgs(T source) {
            this.Source = source;
        }

        public override String ToString() {
            return "EventArgs<" + Source + ">";
        }
    }

    public delegate void EventCallback<T>(T source, EventBaseArgs<T> args);

    public class EventRegister <T> {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(EventRegister<T>));

        public event EventCallback<T> onTrigger;

        public void triggerEvent(T source, EventBaseArgs<T> args) {
            logger.Debug("Trigger event <" + source + "> : [" + args + "]");
            onTrigger(source, args);
        }

        public void resetTriggers() {
            onTrigger = null;
        }
    }
}
