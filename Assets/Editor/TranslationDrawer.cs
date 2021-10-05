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
        public string result2;

        public override void OnInspectorGUI() {
            if (GUILayout.Button("Load Translations")) {
                var path = Path.Combine(Constants.ResourcePath, "translation.csv");
                result = File.ReadAllBytes(path).Encode(Encoding.GetEncoding(51949)).Encode(Encoding.GetEncoding(51949), Encoding.UTF8).Replace("\r", "");
                var csv = new Csv(result);
                var keys = new List<string>();
                var langs = new List<string>();
                var cols = new List<List<string>>();
                var data = csv.Data;
                langs = data[0];
                langs.RemoveAt(0);
                data.RemoveAt(0);
                foreach (var rows in data) {
                    keys.Add(rows[0]);
                    rows.RemoveAt(0);
                    cols.Add(rows);
                }
                
                var datas = new Dictionary<string, Dictionary<SystemLanguage, string>>();
                var col = 0;
                foreach (var key in keys) {
                    datas[key] = new Dictionary<SystemLanguage, string>();
                    var row = 0;
                    foreach (var langstr in langs) {
                        if (Enum.TryParse<SystemLanguage>(langstr.FirstCapital(), out var lang)) {
                            datas[key][lang] = cols[col][row];
                        }

                        row++;
                    }

                    col++;
                }
                
                result2 = Json.Serialize(datas, false);
                Translate.TranslationsDict = datas;
            }

            GUILayout.TextArea(result);
            GUILayout.TextArea(result2);
            GUILayout.Space(10);
            var t = (Translate) target;
            t.currentLanguage = (SystemLanguage) EditorGUILayout.EnumPopup(t.currentLanguage);
        }
    }
}