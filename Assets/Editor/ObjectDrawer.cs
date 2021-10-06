/*
using System;
using Core.Level;
using UnityEditor;
using UnityEngine;
using Utils;
using DataType = Utils.SerializableObject.DataType;

namespace Editor {
    [CustomPropertyDrawer(typeof(SerializableObject))]
    public class ObjectDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            var type = (DataType) property.FindPropertyRelative("type").enumValueIndex;
            var pos1 = new Rect(position.x, position.y, position.width - 80, 20);
            var pos2 = new Rect(position.x + position.width - 80, position.y, 80, 20);
            switch (type) {
                case DataType.BOOL:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("boolData"), GUIContent.none);
                    break;

                case DataType.BYTE:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("byteData"), GUIContent.none);
                    break;

                case DataType.SBYTE:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("sbyteData"), GUIContent.none);
                    break;

                case DataType.CHAR:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("charData"), GUIContent.none);
                    break;

                case DataType.DOUBLE:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("doubleData"), GUIContent.none);
                    break;

                case DataType.FLOAT:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("floatData"), GUIContent.none);
                    break;

                case DataType.INT:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("intData"), GUIContent.none);
                    break;

                case DataType.UINT:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("uintData"), GUIContent.none);
                    break;

                case DataType.LONG:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("longData"), GUIContent.none);
                    break;

                case DataType.ULONG:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("ulongData"), GUIContent.none);
                    break;

                case DataType.SHORT:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("shortData"), GUIContent.none);
                    break;

                case DataType.USHORT:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("ushortData"), GUIContent.none);
                    break;

                case DataType.STRING:
                    EditorGUI.PropertyField(pos1, property.FindPropertyRelative("stringData"), GUIContent.none);
                    break;
            }

            property.FindPropertyRelative("type").enumValueIndex = Convert.ToInt32(EditorGUI.EnumPopup(pos2, type));
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return 50;
        }
    }
}
*/