using System.Collections.Generic;
using UnityEngine;

namespace Locale {
    public class Translate : MonoBehaviour {
        public static SystemLanguage CurrentLanguage;
        public static Dictionary<string, Dictionary<SystemLanguage, string>> TranslationsDict;

        public static bool TryGet(string key, out string value, SystemLanguage? language = null) {
            language ??= CurrentLanguage;
            if (!TranslationsDict.ContainsKey(key)) {
                value = null;
                return false;
            }

            var dict2 = TranslationsDict[key];

            if (dict2.TryGetValue(language.Value, out value)) return true;
            return dict2.TryGetValue(SystemLanguage.English, out value);
        }

        public static string Get(string key, SystemLanguage? language = null) {
            if (TryGet(key, out var value, language)) {
                return value;
            }

            return $"KeyNotFound {key}";
        }
    }
}