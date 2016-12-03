using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.Events {
    public class PropertyChangeArgs<S, T> : EventBaseArgs<S> where T : class {

        public string PropertyName { get; private set; }
        public T PriorValue { get; private set; }
        public T NewValue { get; private set; }

        public PropertyChangeArgs(S source, string name, T oValue, T nValue) : base(source) {
            this.PropertyName = name;
            this.PriorValue = oValue;
            this.NewValue = nValue;
        }

        public PropertyChangeArgs(S source, string name, T nValue) : this (source, name, null, nValue) {}
        public PropertyChangeArgs(S source, string name) : this (source, name, null) {}
    }
}
