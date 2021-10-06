/*
using System;
using Core.Level;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomPropertyDrawer(typeof(NoteType))]
    public class NoteTypeProperty : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var sliderPos = new Rect(position.x, position.y, position.width - 60, position.height);
            var fieldPos = new Rect(position.x + position.width - 50, position.y, 50, position.height);
            var names = Enum.GetNames(typeof(NoteType));
            property.enumValueIndex = GUI.SelectionGrid(position, property.enumValueIndex, names, names.Length);
            EditorGUI.EndProperty();
        }
    }
}
*/