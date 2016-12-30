using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.Events {
    public class EventBaseArgs <TSource> : EventArgs {
        public TSource Source { get; private set; }

        public EventBaseArgs(TSource source) {
            this.Source = source;
        }

        public override String ToString() {
            return "EventArgs<" + Source + ">";
        }
    }

    public delegate void EventCallback<T>(object sender, EventBaseArgs<T> e);

    public class EventRegister <TSource> {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(EventRegister<TSource>));

        public event EventCallback<TSource> onTrigger;

        public void triggerEvent(object source, EventBaseArgs<TSource> args) {
            logger.Debug("Trigger event <" + source + "> : [" + args + "]");
            onTrigger(source, args);
        }

        public void resetTriggers() {
            onTrigger = null;
        }
    }
}
