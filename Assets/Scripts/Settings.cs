using System;
using System.Collections.Generic;
using System.IO;
using Core.Level;
using Serialization;
using UnityEngine;
using Utils;

public static class Settings {
    public static Dictionary<NotePos, KeyCode> Keymap;
    public static Vector2Int ScreenResolution;
    public static FullScreenMode FullScreenMode;

    public static void ApplySettings() {
        Screen.SetResolution(ScreenResolution.x, ScreenResolution.y, FullScreenMode);
    }
    public static void LoadSettings() {
        var path = Path.Combine(Constants.DataPath, "settings.json");
        try {
            if (!File.Exists(path)) DecodeSettings(new Dictionary<string, object>());
            else {
                var data = Json.Deserialize(File.ReadAllText(path));
                DecodeSettings((Dictionary<string, object>) data);
            }
            Debug.Log($"Loaded settings from path {path}");
        } catch (Exception e) {
            Debug.LogWarning($"Cannot load settings from path {path}:\n{e}");
            DecodeSettings(new Dictionary<string, object>());
        }
    }
    public static void SaveSettings() {
        var path = Path.Combine(Constants.DataPath, "settings.json");
        try {
            File.WriteAllText(path, Json.Serialize(EncodeSettings(), false));
            Debug.Log($"Saved settings to path {path}");
        } catch (Exception e) {
            Debug.LogWarning($"Cannot save settings to path {path}:\n{e}");
        }
    }

    public static Dictionary<string, object> EncodeSettings() {
        var dict = new Dictionary<string, object>() {
            {"KeyMap", Keymap},
            {"ScreenResolution", ScreenResolution},
            {"FullscreenMode", FullScreenMode}
        };
        return dict;
    }

    public static void DecodeSettings(Dictionary<string, object> settings) {
        Keymap = settings.GetOrDefault("KeyMap", new Dictionary<NotePos, KeyCode> {
            {NotePos.POS_0, KeyCode.D},
            {NotePos.POS_1, KeyCode.F},
            {NotePos.POS_2, KeyCode.J},
            {NotePos.POS_3, KeyCode.K},
        }).As<Dictionary<NotePos, KeyCode>>();
        
        ScreenResolution = settings.GetOrDefault("ScreenResolution",
            new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height)).As<Vector2Int>();

        FullScreenMode = settings.GetOrDefault("FullscreenMode", FullScreenMode.FullScreenWindow).As<FullScreenMode>();
    }
}