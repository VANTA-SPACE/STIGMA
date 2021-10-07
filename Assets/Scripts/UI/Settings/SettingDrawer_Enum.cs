using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace UI.Settings {
    // ReSharper disable once InconsistentNaming
    public class SettingDrawer_Enum : SettingDrawer {
        public Button buttonLeft;
        public Button buttonRight;
        public TMP_Text text;
        public List<Enum> Options;
        public Type EnumType;
        public override void Init(SettingProperty property) {
            return;
        }
    }
}