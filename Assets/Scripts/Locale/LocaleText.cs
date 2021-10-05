using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Locale {
    public class LocaleText : MonoBehaviour {
        [FormerlySerializedAs("LocaleKey")] public string localeKey;

        public void Awake() {
            var text = GetComponent<Text>();
            if (text != null) text.text = Translate.Get(localeKey);
            var text2 = GetComponent<TMP_Text>();
            if (text2 != null) text2.text = Translate.Get(localeKey);
        }
    }
}