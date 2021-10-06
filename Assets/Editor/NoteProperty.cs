/*
using Core.Level;
using UnityEditor;
using UnityEngine;

namespace Editor {
    //[CustomPropertyDrawer(typeof(NoteData))]
    public class NoteProperty : PropertyDrawer {
        public float height;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var width = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;
            EditorGUI.BeginProperty(position, label, property);
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUIUtility.labelWidth = width;
            //var indent = EditorGUI.indentLevel;
            //EditorGUI.indentLevel = 0;

            var rect0 = new Rect(position.x, position.y + 1, position.width, 20);
            var style = new GUIStyle() {
                richText = true,
                normal = new GUIStyleState {
                    textColor = new Color(0.9f, 0.9f, 0.9f)
                },
                fontSize = 13
            };
            GUI.Label(rect0, "<b>" + label.text.Replace("Element ", "Note #") + "</b>", style);

            var rect1 = new Rect(position.x, position.y + 20, position.width, 20);
            var rect2 = new Rect(position.x, position.y + 45, position.width, 20);
            var rect3 = new Rect(position.x, position.y + 70, position.width, 20);
            var rect4 = new Rect(position.x, position.y + 95, position.width, 20);
            var prop1 = property.FindPropertyRelative("NoteType");
            var prop2 = property.FindPropertyRelative("StartBeat");
            var prop3 = property.FindPropertyRelative("NotePos");
            var prop4 = property.FindPropertyRelative("AdditionalData");
            
            EditorGUI.PropertyField(rect1, prop1, new GUIContent("Note Type"));
            EditorGUI.PropertyField(rect2, prop2, new GUIContent("Start Beat"));
            var value = (NoteType) prop1.enumValueIndex;
            switch (value) {
                case NoteType.Long:
                    EditorGUI.PropertyField(rect3, prop3, new GUIContent("Note Position"));
                    EditorGUI.PropertyField(rect4, prop4, new GUIContent("AdditionalData"));
                    break;
                case NoteType.Normal:
                    EditorGUI.PropertyField(rect3, prop3, new GUIContent("Note Position"));
                    break;
            }

            height = 25 * 5;

            //EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return height;
        }
    }
}
*/