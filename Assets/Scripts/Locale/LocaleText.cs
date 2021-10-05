using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Locale {
    public class LocaleText : MonoBehaviour {
        public string prefix;
        public string localeKey;
        public string suffix;
        public object[] formats;

        public void Awake() {
            UpdateText();
            Events.OnLanguageChange.AddListener(UpdateText);
        }

        public void UpdateText() {
            var text = GetComponent<Text>();
            if (text != null) text.text = prefix + Translate.GetFormatted(localeKey, null, formats) + suffix;
            var text2 = GetComponent<TMP_Text>();
            if (text2 != null) text2.text = prefix + Translate.GetFormatted(localeKey, null, formats) + suffix;
        }
    }
}