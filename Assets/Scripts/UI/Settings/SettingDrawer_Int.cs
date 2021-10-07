using TMPro;
using UnityEngine.UI;

namespace UI.Settings {
    // ReSharper disable once InconsistentNaming
    public class SettingDrawer_Int : SettingDrawer {
        public Button buttonLeft;
        public Button buttonRight;
        public TMP_Text text;
        public int max;
        public int min;
        public override void Init(SettingProperty property) {
            return;
        }
    }
}