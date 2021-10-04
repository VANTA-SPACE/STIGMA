using System.Collections.Generic;
using DebugObj;
using GDMiniJSON;
using Level;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(JsonSerializer))]
    public class JsonSerializerInspector : UnityEditor.Editor {
        private string value = "None";

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            JsonSerializer serializer = (JsonSerializer) target;
            if (GUILayout.Button("Serialize")) {
                value = Json.Serialize(serializer.NoteData.Encode());
            }

            if (GUILayout.Button("Deserialize")) {
                serializer.NoteData = new NoteData((Dictionary<string, object>) Json.Deserialize(value));
                Debug.Log(serializer.NoteData.NoteType);
            }

            value = GUILayout.TextField(value);
        }
    }
}