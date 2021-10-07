using System;
using System.Collections.Generic;
using Locale;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Settings {
    public class SettingProperty : MonoBehaviour {
        public LocaleText propertyName;
        
        public GameObject intPrefab;
        public GameObject enumPrefab;
        public GameObject optionsPrefab;
        public GameObject keymapPrefab;
        [NonSerialized] public string Category;
        [NonSerialized] public string Property;

        [NonSerialized] public SettingDrawer Drawer;

        public void Init(string category, string property) {
            Property = property;
            Category = category;
            
            propertyName.localeKey = $"Settings.{category}.{property}";
            propertyName.UpdateText();
            
            var type = global::Settings.SettingValues[category][property].GetType();
            SettingDrawer drawer;
            
            if (global::Settings.SettingData[Category].GetOrDefault(Property) is Dictionary<string, object> data && data.ContainsKey("options")) {
                drawer = Instantiate(optionsPrefab, transform, false).GetComponent<SettingDrawer_Options>();
            } else if (type == typeof(int)) {
                drawer = Instantiate(intPrefab, transform, false).GetComponent<SettingDrawer_Int>();
            } else if (type == typeof(KeyCode)) {
                drawer = Instantiate(keymapPrefab, transform, false).GetComponent<SettingDrawer_Keymap>();
            } else if (type.IsSubclassOf(typeof(Enum))) {
                drawer = Instantiate(enumPrefab, transform, false).GetComponent<SettingDrawer_Enum>();
            } else return;
            drawer.Init(this);
            drawer.AfterInit();
        }
    }
}