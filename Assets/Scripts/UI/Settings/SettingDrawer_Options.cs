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
        
        public override void Init(SettingProperty property) {
            base.Init(property);
            var data = (Dictionary<string, object>) global::Settings.SettingData[property.Category].GetOrDefault(property.Property);
            Options = data["options"].As<List<object>>();
            allowOverflow = (bool) data.GetOrDefault("allowOverflow", true);
            Debug.Log($"{property.Property}: {allowOverflow}");
            
            buttonLeft.onClick.AddListener(() => {
                var val = Value;
                var index = Options.IndexOf(val);
                if (allowOverflow) index = (index - 1 + Options.Count) % Options.Count;
                else index = Math.Max(index - 1, 0);
                Debug.Log(Options[index]);
                val = Options[index];
                Value = val;
            });
            
            buttonRight.onClick.AddListener(() => {
                var val = Value;
                var index = Options.IndexOf(val);
                if (allowOverflow) index = (index + 1 + Options.Count) % Options.Count;
                else index = Math.Min(index + 1, Options.Count - 1);
                Debug.Log(Options[index]);
                val = Options[index];
                Value = val;
            });
        }

        public override string GetText(object value) {
            if (Property.Property == "TargetFrameRate" && value.Equals(10000)) {
                return Translate.Get("Settings.Graphic.TargetFrameRate.Unlimited");
            }

            return value switch {
                Vector2Int vector => $"{vector.x}×{vector.y}",
                Language language => 
                    Translate.TryGet("Language.Name", out string langname, language) ? langname : language.ToString(),
                Enum enum2 => enum2.GetEnumName(),
                _ => value.ToString()
            };
        }
    }
}