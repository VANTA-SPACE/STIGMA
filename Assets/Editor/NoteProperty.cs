using Core.Level;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomPropertyDrawer(typeof(NoteData))]
    public class NoteProperty : PropertyDrawer {
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
            var prop1 = property.FindPropertyRelative("StartBeat");
            var prop2 = property.FindPropertyRelative("NotePos");
            
            EditorGUI.PropertyField(rect1, prop1, new GUIContent("Start Beat"));
            EditorGUI.PropertyField(rect2, prop2, new GUIContent("Note Position"));

            //EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return 80;
        }
    }
}