using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;

namespace com.kintoshmalae.SFXEngine.I18N {
    public sealed class I18NString {
        private I18NString() {}

        public static string Lookup(string id) {
            ResourceManager rm = new ResourceManager("Messages", typeof(I18NString).Assembly);
            return rm.GetString(id);
        }
    }
}
