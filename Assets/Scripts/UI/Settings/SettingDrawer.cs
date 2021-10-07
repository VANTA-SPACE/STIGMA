using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Settings {
    public abstract class SettingDrawer : MonoBehaviour {
        [NonSerialized] public SettingProperty Property;
        public Button buttonLeft;
        public Button buttonRight;
        public TMP_Text text;

        public object Value {
            get => global::Settings.SettingValues[Property.Category][Property.Property];
            set => global::Settings.SettingValues[Property.Category][Property.Property] = value;
        }

        public virtual void Init(SettingProperty property) {
            Property = property;
        }

        public virtual void AfterInit() {
            if (global::Settings.SettingData[Property.Category].GetOrDefault(Property.Property) is Dictionary<string, object> data) {
                if (data.GetOrDefault("refresh", false).As(false)) {
                    buttonLeft.onClick.AddListener(global::Settings.ApplySettings);
                    buttonRight.onClick.AddListener(global::Settings.ApplySettings);
                }
            }
            buttonLeft.onClick.AddListener(UpdateText);
            buttonRight.onClick.AddListener(UpdateText);
            UpdateText();
        }

        public abstract string GetText(object value);
        public void UpdateText() => text.text = GetText(Value);
    }
}