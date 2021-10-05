using System.Collections.Generic;
using Core.Level;
using Manager;
using Serialization;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(PlayManager)), CanEditMultipleObjects]
    public class PlayerDrawer : UnityEditor.Editor {
        public static string Result = "";
        public static Vector2 Position = Vector2.zero;
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var level = (PlayManager) target;
            if (GUILayout.Button("Build Level")) {
                Result = Json.Serialize(level.levelData.Encode());
            }

            GUILayout.Space(10);
            Position = EditorGUILayout.BeginScrollView(Position);
            Result = EditorGUILayout.TextArea(Result);
            EditorGUILayout.EndScrollView();
        
            if (GUILayout.Button("Apply Level")) {
                level.levelData = new LevelData((Dictionary<string, object>) Json.Deserialize(Result));
            }
        }
    }
}