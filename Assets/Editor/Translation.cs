/*
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Level;
using Locale;
using Manager;
using NUnit.Framework;
using Serialization;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor {
    [CustomEditor(typeof(LocaleText)), CanEditMultipleObjects]
    public class Translation : UnityEditor.Editor {
        public string result;
        public static Language Language = Language.English;

        public List<Language> langs;

        public override void OnInspectorGUI() {
            if (langs == null) {
                langs = Translate.AvailableLanguages;
                Language = Language.English;
            }
            
            var curridx = langs.IndexOf(Language);
            var idx = EditorGUILayout.Popup(curridx, langs.Select(lang => lang.ToString()).ToArray());
            base.OnInspectorGUI();
            if (idx != curridx) {
                var text = (LocaleText) target;
                text.UpdateText(Language);
                Debug.Log(text);
                Language = langs[idx];
            }
        }
    }
}
*/