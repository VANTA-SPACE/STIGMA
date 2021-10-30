using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Manager;
using Serialization;
using UnityEngine;
using Utils;

namespace Locale {
    public class Translate : MonoBehaviour {
        public static Dictionary<string, Dictionary<Language, string>> TranslationsDict;

        public static List<Language> AvailableLanguages {
            get {
                if (_availableLanguages == null) LoadLanguages();
                return _availableLanguages;
            }
        }
        private static List<Language> _availableLanguages;

        private static (string, string)[] leetTable = {
            ("A", "4"),
            ("B", "|3"),
            ("C", "("),
            ("D", "[)"),
            ("E", "3"),
            ("F", "|="),
            ("G", "6"),
            ("H", "|-|"),
            ("I", "]["),
            ("J", "_]"),
            ("K", "|{"),
            ("L", "1"),
            ("N", "/|/"),
            ("O", "()"),
            ("P", "|^"),
            ("Q", "&"),
            ("R", "|?"),
            ("S", "5"),
            ("T", "7"),
            ("V", "\\|"),
            ("W", "|^|"),
            ("X", "><"),
            ("Y", "'/"),
            ("Z", "2"),
            
            ("M", "|V|"),
            ("U", "L|"),
        };

        public static bool TryGet(string key, out string value, Language? language = null) {
            if (TranslationsDict == null) TranslationsDict = LoadLanguages();
            language ??= Settings.CurrentLanguage;
            if (!TranslationsDict.ContainsKey(key)) {
                value = null;
                return false;
            }

            var dict2 = TranslationsDict[key];

            if (language == Language.LEET) {
                bool result = dict2.TryGetValue(Language.English, out var engValue);
                if (!result) {
                    value = null;
                    return false;
                }

                foreach (var (eng, leet) in leetTable) {
                    engValue = engValue.Replace(eng.ToUpper(), leet);
                    engValue = engValue.Replace(eng.ToLower(), leet);
                }

                value = engValue;
                return true;
            }
            if (dict2.TryGetValue(language.Value, out value)) return true;
            return dict2.TryGetValue(Language.English, out value);
        }

        public static string Get(string key, Language? language = null) =>
            TryGet(key, out string value, language) ? value : $"KeyNotFound {key}";

        public static bool TryGetFormatted(string key, out string value, Language? language = null, params object[] formats) {
            if (formats == null) return TryGet(key, out value, language);
            if (!TryGet(key, out value, language)) return false;
            try {
                value = string.Format(value, formats);
            } catch { }
            return true;
        }

        public static string GetFormatted(string key, Language? language = null, params object[] formats) =>
            TryGetFormatted(key, out string value, language, formats) ? value : $"KeyNotFound {key}";

        public static Dictionary<string, Dictionary<Language, string>> LoadLanguages() {
            _availableLanguages = new List<Language>();
            string result = string.Empty;
            if (Application.isEditor && Application.isPlaying) {
#if  UNITY_EDITOR
                var path = Path.Combine(Constants.ResourcePath, "translation.csv");
                try {
                    using var client = new WebClient();
                    client.DownloadFile(
                        "https://docs.google.com/spreadsheets/d/14LI14cXLixkUbz1Ap4UzH1hRZkZCKbd6ysrvhExC_Mo/export?format=csv",
                        path);
                } catch { }

                File.Delete(path + ".tmp");
                File.Copy(path, path + ".tmp");
                path = path + ".tmp";
                result = File.ReadAllBytes(path).Encode(Encoding.GetEncoding(65001)).Replace("\r", "");
                File.Delete(path);
#endif
            } else {
                if (!Application.isPlaying) Debug.Log("Not loading sheet due to not playing ");
                result = Resources.Load<TextAsset>("translation").text.Replace("\r", "");
            }
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

            var datas = new Dictionary<string, Dictionary<Language, string>>();
            var col = 0;
            foreach (var key in keys) {
                datas[key] = new Dictionary<Language, string>();
                var row = 0;
                foreach (var langstr in langs) {
                    if (Enum.TryParse<Language>(langstr, out var lang)) {
                        _availableLanguages.Add(lang);
                        if (lang == Language.English) _availableLanguages.Add(Language.LEET);
                        datas[key][lang] = cols[col][row];
                    }

                    row++;
                }

                col++;
            }
            Debug.Log("End loaded");
            return datas;
        }
    }
}