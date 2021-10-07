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
    public class SettingDrawer_Boolean : SettingDrawer {
        public override void Init(SettingProperty property) {
            base.Init(property);
            buttonLeft.onClick.AddListener(() => {
                Value = !(bool) Value;
            });
            
            buttonRight.onClick.AddListener(() => {
                Value = !(bool) Value;
            });
        }

        public override string GetText(object value) {
            return (bool) value ? Translate.Get("Settings.True") : Translate.Get("Settings.False");
        }
    }
}