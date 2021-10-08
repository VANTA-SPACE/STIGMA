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
    public class SettingDrawer_Enum : SettingDrawer {
        public List<Enum> Options;
        public Type EnumType;
        public override void Init(SettingProperty property) {
            base.Init(property);
            var value = Value;
            EnumType = value.GetType();
            if (!EnumType.IsSubclassOf(typeof(Enum))) throw new Exception($"{EnumType} is not enum!");
            Options = Enum.GetValues(EnumType).Cast<Enum>().ToList();
            Debug.Log(Json.Serialize(Options));
            var key = EnumType.Name;
            
            buttonLeft.onClick.AddListener(() => {
                var val = (Enum) Value;
                var index = Options.IndexOf(val);
                index = (index - 1 + Options.Count) % Options.Count;
                Debug.Log(Options[index]);
                val = Options[index];
                Value = val;
            });
            
            buttonRight.onClick.AddListener(() => {
                var val = (Enum) Value;
                var index = Options.IndexOf(val);
                index = (index + 1 + Options.Count) % Options.Count;
                Debug.Log(Options[index]);
                val = Options[index];
                Value = val;
            });
        }

        public override string GetText(object value) {
            if (value is Language language) {
                if (language == Language.LEET) return "1337";
                return Translate.TryGet("Language.Name", out var langname, language) ? langname : language.ToString();
            } else return ((Enum) value).GetEnumName();
        }
    }
}