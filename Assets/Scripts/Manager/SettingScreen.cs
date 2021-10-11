using System;
using System.Collections.Generic;
using UI.Settings;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Manager {
    public class SettingScreen : Manager<SettingScreen> {
        public static UnityEvent UpdateProps = new UnityEvent();
        private string currentCategory = "General";

        public string CurrentCategory {
            get => currentCategory;
            set {
                currentCategory = value;
                UpdateCategory();
            }
        }
        
        public Dictionary<string, List<SettingProperty>> Properties;
        public GameObject propertyPrefab;
        public Transform propertyParent;
        [NonSerialized] public SettingDrawer_Keymap CurrentKeymap;

        public override void Init() {
            UpdateProps.RemoveAllListeners();
            Properties = new Dictionary<string, List<SettingProperty>>();
            foreach ((string category, var values) in Settings.SettingValues) {
                Properties[category] = new List<SettingProperty>();
                foreach ((string key, object value) in values) {
                    var prop = Instantiate(propertyPrefab, propertyParent, false).GetComponent<SettingProperty>();
                    prop.Init(category, key);
                    Properties[category].Add(prop);
                }
            }
            
            UpdateCategory();
        }

        public UnityEvent OnUpdateCategory = new UnityEvent();
        public void UpdateCategory() {
            CurrentKeymap = null;
            foreach (var (category, properties) in Properties) {
                var value = category == CurrentCategory;
                foreach (var property in properties) {
                    property.gameObject.SetActive(value);
                }
            }
            
            OnUpdateCategory.Invoke();
        }

        private void Update() {
            if (CurrentKeymap != null) CurrentKeymap.CheckKey();
        }
    }
}