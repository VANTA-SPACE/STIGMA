using System;
using Core.Level;
using Locale;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomPropertyDrawer(typeof(Translation))]
    public class TranslateEditor : PropertyDrawer {
        public int length;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            Rect pos = new Rect(position.x, position.y, 100, 20);
            Rect pos2 = new Rect(position.x + 105, position.y, position.width - 100, 20);
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("translationKey"), GUIContent.none);
            var arrayProp = property.FindPropertyRelative("translations");
            length = arrayProp.arraySize;
            
            for (int i = 0; i < arrayProp.arraySize; i++) {
                SerializedProperty value = arrayProp.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(pos2, value, GUIContent.none);
                pos2.y += 25;
            }
            if (GUI.Button(pos2, "+")) {
                arrayProp.InsertArrayElementAtIndex(length);
            }


            //EditorGUI.PropertyField(position, );
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return Mathf.Max(length * 25, 25);
        }
    }
}