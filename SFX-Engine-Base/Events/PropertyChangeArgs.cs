using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.SFXEngine.Events {
    public class PropertyChangeArgs<TSource, TValue> : EventBaseArgs<TSource> where TValue : class {

        public string PropertyName { get; private set; }
        public TValue PriorValue { get; private set; }
        public TValue NewValue { get; private set; }

        public PropertyChangeArgs(TSource source, string name, TValue oValue, TValue nValue) : base(source) {
            this.PropertyName = name;
            this.PriorValue = oValue;
            this.NewValue = nValue;
        }

        public PropertyChangeArgs(TSource source, string name, TValue nValue) : this (source, name, null, nValue) {}
        public PropertyChangeArgs(TSource source, string name) : this (source, name, null) {}

        public override String ToString() {
            return PropertyName + ":[" + PriorValue + "]->[" + NewValue + "]";
        }
    }
}
