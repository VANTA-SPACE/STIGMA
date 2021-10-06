/*
using Core.Level;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor {
    [CustomPropertyDrawer(typeof(SerializableKeyValuePair<,>))]
    public class PairDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            var pos1 = new Rect(position.x, position.y, position.width / 2 - 3, position.height);
            var pos2 = new Rect(position.x + position.width / 2 + 3, position.y, position.width / 2 - 3, position.height);
            
            EditorGUI.PropertyField(pos1, property.FindPropertyRelative("Key"), GUIContent.none);
            EditorGUI.PropertyField(pos2, property.FindPropertyRelative("Value"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}
*/