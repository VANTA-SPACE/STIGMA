/*
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
            var names = new[] {"1", "2", "3", "4"};
            property.enumValueIndex = GUI.SelectionGrid(position, property.enumValueIndex, names, names.Length);
            EditorGUI.EndProperty();
        }
    }
}
*/