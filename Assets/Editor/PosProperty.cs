using System;
using Core.Level;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomPropertyDrawer(typeof(NotePos))]
    public class PosProperty : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var sliderPos = new Rect(position.x, position.y, position.width - 60, position.height);
            var fieldPos = new Rect(position.x + position.width - 50, position.y, 50, position.height);
            var max = Enum.GetNames(typeof(NotePos)).Length - 1;
            property.enumValueIndex = (int) (GUI.HorizontalSlider(sliderPos, property.enumValueIndex, 0, max) + 0.5f);
            var idx = (EditorGUI.IntField(fieldPos, property.enumValueIndex));
            if (idx > max) idx = max;
            if (idx < 0) idx = 0;
            property.enumValueIndex = idx;
            EditorGUI.EndProperty();
        }
    }
}