using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Level;
using Locale;
using Manager;
using NUnit.Framework;
using Serialization;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor {
    [CustomEditor(typeof(LocaleText)), CanEditMultipleObjects]
    public class Translation : UnityEditor.Editor {
        public string result;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var text = (LocaleText) target;
            text.UpdateText();
        }
    }
}