using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serialization;
using UnityEngine;
using Utils;

namespace Locale {
    public class Translate : MonoBehaviour {
        public static SystemLanguage CurrentLanguage {
            get => _currentLanguage;
            set {
                _currentLanguage = value;
                Events.OnLanguageChange.Invoke();
            }
        }
        private static SystemLanguage _currentLanguage;
        public static Dictionary<string, Dictionary<SystemLanguage, string>> TranslationsDict;
        public SystemLanguage currentLanguage;

        private void Awake() {
            currentLanguage = Application.systemLanguage;
            TranslationsDict = LoadLanguages();
        }

        private void Update() {
            CurrentLanguage = currentLanguage;
        }

        public static bool TryGet(string key, out string value, SystemLanguage? language = null) {
            if (TranslationsDict == null) TranslationsDict = LoadLanguages();
            language ??= CurrentLanguage;
            if (!TranslationsDict.ContainsKey(key)) {
                value = null;
                return false;
            }

            var dict2 = TranslationsDict[key];

            if (dict2.TryGetValue(language.Value, out value)) return true;
            return dict2.TryGetValue(SystemLanguage.English, out value);
        }

        public static string Get(string key, SystemLanguage? language = null) =>
            TryGet(key, out string value, language) ? value : $"KeyNotFound {key}";

        public static bool TryGetFormatted(string key, out string value, SystemLanguage? language = null,
            params object[] formats) {
            if (formats == null) return TryGet(key, out value, language);
            if (!TryGet(key, out value, language)) return false;
            value = string.Format(value, formats);
            return true;
        }

        public static string GetFormatted(string key, SystemLanguage? language = null, params object[] formats) =>
            TryGetFormatted(key, out string value, language, formats) ? value : $"KeyNotFound {key}";

        public static Dictionary<string, Dictionary<SystemLanguage, string>> LoadLanguages() {
            #if UNITY_EDITOR
            var path = Path.Combine(Constants.ResourcePath, "translation.csv");
            File.Delete(path + ".tmp");
            File.Copy(path, path + ".tmp");
            path = path + ".tmp";
            var result = File.ReadAllBytes(path).Encode(Encoding.GetEncoding(65001)).Replace("\r", "");
            File.Delete(path);
            #else
            var result = Resources.Load<TextAsset>("translation").text.Replace("\r", "");
            #endif
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

            return datas;
        }
    }
}