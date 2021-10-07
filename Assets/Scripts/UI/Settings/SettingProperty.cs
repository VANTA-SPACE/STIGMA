using System;
using UnityEngine;

namespace UI.Settings {
    public class SettingProperty : MonoBehaviour {
        public GameObject intPrefab;
        public GameObject enumPrefab;
        [NonSerialized] public string Category;
        [NonSerialized] public string Property;

        [NonSerialized] public SettingDrawer Drawer;

        public void Init(string category, string property) {
            Category = category;
            Property = property;
            var type = global::Settings.SettingValues[category][property].GetType();
            SettingDrawer drawer;
            if (type == typeof(int)) {
                drawer = Instantiate(intPrefab, transform, false).GetComponent<SettingDrawer_Int>();
            } else if (type.IsSubclassOf(typeof(Enum))) {
                drawer = Instantiate(intPrefab, transform, false).GetComponent<SettingDrawer_Enum>();
            } else return;
            drawer.Init(this);
        }
    }
}