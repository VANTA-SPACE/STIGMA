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
        public GameObject booleanPrefab;
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

            var value = global::Settings.SettingValues[category][property];
            SettingDrawer drawer;
            
            if (global::Settings.SettingData[Category].GetOrDefault(Property) is Dictionary<string, object> data && data.ContainsKey("options")) {
                drawer = Instantiate(optionsPrefab, transform, false).GetComponent<SettingDrawer_Options>();
            } else {
                switch (value) {
                    case int _:
                        drawer = Instantiate(intPrefab, transform, false).GetComponent<SettingDrawer_Int>();
                        break;
                    case bool _:
                        drawer = Instantiate(booleanPrefab, transform, false).GetComponent<SettingDrawer_Boolean>();
                        break;
                    case KeyCode _:
                        drawer = Instantiate(keymapPrefab, transform, false).GetComponent<SettingDrawer_Keymap>();
                        break;
                    case Enum _:
                        drawer = Instantiate(enumPrefab, transform, false).GetComponent<SettingDrawer_Enum>();
                        break;
                    default:
                        return;
                }
            }
            drawer.Init(this);
            drawer.AfterInit();
        }
    }
}