using System.Collections.Generic;
using UnityEngine;

namespace Locale {
    public class Translate : MonoBehaviour {
        public static SystemLanguage CurrentLanguage;
        private static Dictionary<string, Dictionary<SystemLanguage, string>> translationsDict;

        public List<Translation> Translations;
        
    }
}