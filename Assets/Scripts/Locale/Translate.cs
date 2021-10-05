using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Locale {
    public class Translate : MonoBehaviour {
        public static SystemLanguage CurrentLanguage;
        public static Dictionary<string, Dictionary<SystemLanguage, string>> TranslationsDict;
        public SystemLanguage currentLanguage = Application.systemLanguage;

        private void Awake() {
            var result = Resources.Load<TextAsset>("translation").text.Replace("\r", "");
            var keys = new List<string>();
            var rawrows = result.Split('\n').ToList();
            var langs = rawrows[0].Split(new []{"\t"}, StringSplitOptions.RemoveEmptyEntries);
            rawrows.RemoveAt(0);
            var rows = new List<List<string>>();
            foreach (var colstr in rawrows) {
                var cols = colstr.Split('\t').ToList();
                if (cols[0] != "") keys.Add(cols[0]);
                cols.RemoveAt(0);
                rows.Add(cols);
            }

            var datas = new Dictionary<string, Dictionary<SystemLanguage, string>>();
            var rowc = 0;
            foreach (var key in keys) {
                var colc = 0;
                datas[key] = new Dictionary<SystemLanguage, string>();
                foreach (var langst in langs) {
                    var langstra = langst.ToLower().ToCharArray();
                    langstra[0] = langstra[0].ToString().ToUpper()[0];
                    var langstr = new string((char[]) langstra);
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

            Translate.TranslationsDict = datas;
        }

        private void Update() {
            CurrentLanguage = currentLanguage;
        }

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