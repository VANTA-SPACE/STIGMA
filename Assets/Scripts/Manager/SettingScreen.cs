using System;
using System.Collections.Generic;
using UI.Settings;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Manager {
    public class SettingScreen : MonoBehaviour {
        public static SettingScreen Instance { get; private set; }
        
        private string _currentCategory = "General";

        public string currentCategory {
            get => _currentCategory;
            set {
                _currentCategory = value;
                UpdateCategory();
            }
        }
        
        public Dictionary<string, List<SettingProperty>> Properties;
        public GameObject propertyPrefab;
        public Transform propertyParent;
        [NonSerialized] public SettingDrawer_Keymap CurrentKeymap;

        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
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
            foreach (var (category, properties) in Properties) {
                var value = category == currentCategory;
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