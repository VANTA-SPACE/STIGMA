using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Level;
using Locale;
using Manager;
using MiniJSON;
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
                result = Resources.Load<TextAsset>("translation").text.Replace("\r", "");
                var keys = new List<string>();
                var rawrows = result.Split('\n').ToList();
                var langs = rawrows[0].Split(new []{"\t"}, StringSplitOptions.RemoveEmptyEntries);
                rawrows.RemoveAt(0);
                var rows = new List<List<string>>();
                foreach (var colstr in rawrows) {
                    var cols = colstr.Split('\t').ToList();
                    if (cols[0] != "") keys.Add(cols[0]);
                    cols.RemoveAt(0);
                    Debug.Log(Json.Serialize(cols));
                    rows.Add(cols);
                }

                var datas = new Dictionary<string, Dictionary<SystemLanguage, string>>();
                Debug.Log(Json.Serialize(keys));
                Debug.Log(Json.Serialize(langs));
                Debug.Log(Json.Serialize(rows));
                var rowc = 0;
                foreach (var key in keys) {
                    var colc = 0;
                    datas[key] = new Dictionary<SystemLanguage, string>();
                    foreach (var langst in langs) {
                        var langstra = langst.ToLower().ToCharArray();
                        langstra[0] = langstra[0].ToString().ToUpper()[0];
                        var langstr = new string(langstra);
                        if (Enum.TryParse<SystemLanguage>(langstr, out var lang)) {
                            datas[key][lang] = rows[rowc][colc];
                            Debug.Log($"datas[{key}][{lang}] = {rows[rowc][colc]}");
                        } else {
                            Debug.Log($"cannot parse {langstr}");
                        }

                        colc++;
                    }

                    rowc++;
                }

                result2 = Json.Serialize(datas);
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