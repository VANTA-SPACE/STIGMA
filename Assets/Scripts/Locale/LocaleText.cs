using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Locale {
    public class LocaleText : MonoBehaviour {
        public string prefix;
        public string localeKey;
        public string suffix;
        public string[] formats;

        public void Start() {
            UpdateText();
            Events.OnLanguageChange.AddListener(() => UpdateText());
        }

        public void UpdateText(Language? language = null) {
            var text = GetComponent<Text>();
            if (text != null) text.text = prefix + Translate.GetFormatted(localeKey, language, formats?.Cast<object>()) + suffix;
            var text2 = GetComponent<TMP_Text>();
            if (text2 != null) text2.text = prefix + Translate.GetFormatted(localeKey, language, formats?.Cast<object>()) + suffix;
        }
    }
}