using System.Collections.Generic;
using Core.Level;
using DebugObj;
using MiniJSON;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(LevelEditor)), CanEditMultipleObjects]
public class LevelDrawer : Editor {
    public static string Result = "";
    public static Vector2 Position = Vector2.zero;
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var level = (LevelEditor) target;
        
        if (GUILayout.Button("Build Level")) {
            Result = Json.Serialize(level.LevelData.Encode());
        }

        GUILayout.Space(10);
        Position = EditorGUILayout.BeginScrollView(Position);
        Result = EditorGUILayout.TextArea(Result);
        EditorGUILayout.EndScrollView();
        
        if (GUILayout.Button("Apply Level")) {
            level.LevelData = new LevelData((Dictionary<string, object>) Json.Deserialize(Result));
        }
    }
}