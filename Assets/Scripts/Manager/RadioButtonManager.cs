using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Manager {
    public class RadioButtonManager : Manager<RadioButtonManager> {
        public Dictionary<string, int> Values = new Dictionary<string, int>();
        public Dictionary<string, UnityEvent> TargetGraphics = new Dictionary<string, UnityEvent>();
    }
}