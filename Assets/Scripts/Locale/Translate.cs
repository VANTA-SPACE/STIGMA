using System.Collections.Generic;
using UnityEngine;

namespace Locale {
    public class Translate {
        public static SystemLanguage CurrentLanguage;
        public static Dictionary<SystemLanguage, Dictionary<string, string>> translationsDict;
    }
}