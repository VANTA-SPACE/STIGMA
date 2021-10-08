using System;
using System.Collections.Generic;
using System.Linq;
using Locale;
using Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Settings {
    // ReSharper disable once InconsistentNaming
    public class SettingDrawer_Options : SettingDrawer {
        public List<object> Options;
        public bool allowOverflow;
        public int countNormal;
        public int countHidden;
        
        public override void Init(SettingProperty property) {
            base.Init(property);
            var data = (Dictionary<string, object>) global::Settings.SettingData[property.Category].GetOrDefault(property.Property);
            Options = data["options"].As<List<object>>();
            countNormal = Options.Count;
            if (data.ContainsKey("hiddens")) {
                Options = Options.Concat(data["hiddens"].As<List<object>>()).ToList();
            }
            countHidden = Options.Count;
            allowOverflow = (bool) data.GetOrDefault("allowOverflow", true);
            Debug.Log($"{property.Property}: {allowOverflow}");
            if (property.Property == "ScreenResolution") Options = Screen.resolutions.Cast<object>().ToList();
            
            buttonLeft.onClick.AddListener(() => {
                var val = Value;
                var index = Options.IndexOf(val);
                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
                    if (allowOverflow) index = (index - 1 + countHidden) % countHidden;
                    else index = Math.Max(index - 1, 0);
                } else {
                    if (allowOverflow) index = (index - 1 + countNormal) % countNormal;
                    else index = Math.Max(index - 1, 0);
                }
                Debug.Log(Options[index]);
                val = Options[index];
                Value = val;
            });
            
            buttonRight.onClick.AddListener(() => {
                var val = Value;
                var index = Options.IndexOf(val);
                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
                    if (allowOverflow) index = (index + 1 + countHidden) % countHidden;
                    else index = Math.Min(index + 1, countHidden - 1);
                } else {
                    if (allowOverflow) index = (index + 1 + countNormal) % countNormal;
                    else index = Math.Min(index + 1, countNormal - 1);
                }
                Debug.Log(Options[index]);
                val = Options[index];
                Value = val;
            });
        }

        public override string GetText(object value) {
            if (Property.Property == "TargetFrameRate" && value.Equals(10000)) {
                return Translate.Get("Settings.General.TargetFrameRate.Unlimited");
            }
            if (!(value is Resolution) && Options.IndexOf(value) >= countNormal) {
                text.color = new Color32(255, 255, 192, 255);
            } else {
                text.color = Color.white;
            }

            return value switch {
                Resolution resolution => $"{resolution.width}×{resolution.height} @ {resolution.refreshRate}hz",
                Language language => 
                    language == Language.LEET ? "1337"
                     : Translate.TryGet("Language.Name", out string langname, language) ? langname : language.ToString(),
                Enum enum2 => enum2.GetEnumName(),
                _ => value.ToString()
            };
        }
    }
}