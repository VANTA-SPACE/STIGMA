using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Locale {
    [Serializable]
    public struct Translation {
        [Serializable]
        public struct TranslationValue {
            public SystemLanguage Language;
            public string Value;
        }
        public string translationKey;
        public List<TranslationValue> translations;

        public Translation(string translationKey) {
            this.translationKey = translationKey;
            this.translations = new List<TranslationValue>();
        }
    }
}