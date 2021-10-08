using System;
using System.Collections.Generic;
using Manager;
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
            get => global::Settings.SettingValuesTemp[Property.Category][Property.Property];
            set => global::Settings.SettingValuesTemp[Property.Category][Property.Property] = value;
        }

        public virtual void Init(SettingProperty property) {
            Property = property;
        }

        public virtual void AfterInit() {
            buttonLeft.onClick.AddListener(UpdateText);
            buttonRight.onClick.AddListener(UpdateText);
            SettingScreen.UpdateProps.AddListener(UpdateText);
            UpdateText();
        }

        public abstract string GetText(object value);
        public void UpdateText() => text.text = GetText(Value);
    }
}