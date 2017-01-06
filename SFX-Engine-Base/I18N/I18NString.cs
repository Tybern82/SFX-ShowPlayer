using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using SecondLanguage;

namespace com.kintoshmalae.SFXEngine.I18N {
    public sealed class I18NString {
        private static readonly I18NString Instance = new I18NString();

        private readonly Translator T = Translator.Default;
        private ResourceManager rm;

        private I18NString() {
            T.RegisterTranslationsByCulture(@"I18N\{0}\*.po");
            T.RegisterTranslation(@"I18N\en-au\sfx-showplayer.po");
        }

        string LookupRM(string id) {
            // Lazy load the ResourceManager, but only load it once.
            if (rm == null) rm = new ResourceManager("Messages", typeof(I18NString).Assembly);
            return (rm != null) ? rm.GetString(id) : null;
        }

        string LookupRM(string id, params string[] p) {
            return String.Format(CultureInfo.CurrentCulture, LookupRM(id), p);
        }

        string LookupPO(string id) {
            return T[id];
        }

        string LookupPO(string id, params string[] p) {
            return T[id, p];
        }

        public static string Lookup(string id) {
            string _result = Instance.LookupPO(id);
            if (_result == null) _result = Instance.LookupRM(id);
            return _result;
        }

        public static string LookupWithParameters(string id, params string[] p) {
            string _result = Instance.LookupPO(id, p);
            if (_result == null) _result = Instance.LookupRM(id, p);
            return _result;
        }
    }
}
