using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Settings {
    // ReSharper disable once InconsistentNaming
    public class SettingDrawer_Int : SettingDrawer {
        [NonSerialized] public int max;
        [NonSerialized] public int min;
        [NonSerialized] public int changeby;
        public override void Init(SettingProperty property) {
            base.Init(property);
            var value = Value;
            min = 0;
            max = int.MaxValue;
            changeby = 1;
            if (global::Settings.SettingData[property.Category].GetOrDefault(property.Property) is Dictionary<string, object> data) {
                min = data.GetOrDefault("min", min).As(min);
                max = data.GetOrDefault("max", max).As(max);
                changeby = data.GetOrDefault("changeby", changeby).As(changeby);
            }
            
            Debug.Log($"Min: {min} Max: {max} Changeby: {changeby}");
            
            buttonLeft.onClick.AddListener(() => {
                int val = (int) Value;
                val -= Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? Math.Max(changeby / 10, 1) : changeby;
                Value = (int) Math.Min(Math.Max(val, min), max);
            });
            
            buttonRight.onClick.AddListener(() => {
                int val = (int) Value;
                val += Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? Math.Max(changeby / 10, 1) : changeby;
                Value = (int) Math.Min(Math.Max(val, min), max);
            });
        }

        public override string GetText(object value) {
            return value.ToString();
        }
    }
}