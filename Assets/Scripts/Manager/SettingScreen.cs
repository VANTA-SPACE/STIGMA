using System;
using System.Collections.Generic;
using UI.Settings;
using UnityEngine;
using Utils;

namespace Manager {
    public class SettingScreen : MonoBehaviour {
        public Dictionary<string, List<SettingProperty>> Properties;
        public GameObject propertyPrefab;
        public Transform propertyParent;

        private void Awake() {
            Properties = new Dictionary<string, List<SettingProperty>>();
            foreach ((string category, var values) in Settings.SettingValues) {
                Properties[category] = new List<SettingProperty>();
                foreach ((string key, object value) in values) {
                    var prop = Instantiate(propertyPrefab, propertyParent, false).GetComponent<SettingProperty>();
                    prop.Init(category, key);
                    Properties[category].Add(prop);
                }
            }
        }
    }
}