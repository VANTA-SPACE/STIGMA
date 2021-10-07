using System;
using UnityEngine;

namespace UI.Settings {
    public abstract class SettingDrawer : MonoBehaviour {
        [NonSerialized] public SettingProperty Property;
        
        public abstract void Init(SettingProperty property);
    }
}