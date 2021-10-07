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
    [CustomEditor(typeof(Translate)), CanEditMultipleObjects]
    public class TranslationDrawer : UnityEditor.Editor {
        public string result;

        public override void OnInspectorGUI() {
            if (GUILayout.Button("Load Translations")) {
                Translate.TranslationsDict = Translate.LoadLanguages();
                result = Json.Serialize(Translate.TranslationsDict, false);
            }

            GUILayout.TextArea(result);
            GUILayout.Space(10);
            var t = (Translate) target;
            var langs = Translate.AvailableLanguages;
            var curridx = langs.IndexOf(Settings.CurrentLanguage);
            var idx = EditorGUILayout.Popup(curridx, langs.Select(lang => lang.ToString()).ToArray());
            Settings.CurrentLanguage = langs[idx];
            if (idx != curridx) Settings.ApplySettings();
        }
    }
}