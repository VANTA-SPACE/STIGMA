using System;
using System.Collections.Generic;
using System.IO;
using Core.Level;
using Locale;
using Serialization;
using UnityEngine;
using UnityEngine.Events;
using Utils;

public static class Settings {
    public static Dictionary<string, object> GeneralSettings {
        get {
            if (!SettingValues.ContainsKey("General")) {
                SettingValues["General"] = new Dictionary<string, object>();
            }

            return SettingValues["General"];
        }
    }
    public static Dictionary<string, object> AudioSettings {
        get {
            if (!SettingValues.ContainsKey("Audio")) {
                SettingValues["Audio"] = new Dictionary<string, object>();
            }

            return SettingValues["Audio"];
        }
    }
    public static Dictionary<string, object> GraphicSettings {
        get {
            if (!SettingValues.ContainsKey("Graphic")) {
                SettingValues["Graphic"] = new Dictionary<string, object>();
            }

            return SettingValues["Graphic"];
        }
    }
    
    public static Dictionary<string, object> GameSettings {
        get {
            if (!SettingValues.ContainsKey("Game")) {
                SettingValues["Game"] = new Dictionary<string, object>();
            }

            return SettingValues["Game"];
        }
    }

    public static Vector2Int ScreenResolution {
        get => (Vector2Int) GraphicSettings["ScreenResolution"];
        set => GraphicSettings["ScreenResolution"] = value;
    }
    public static FullScreenMode FullScreenMode {
        get => (FullScreenMode) GraphicSettings["FullScreenMode"];
        set => GraphicSettings["FullScreenMode"] = value;
    }
    public static int TargetFrameRate {
        get => (int) GraphicSettings["TargetFrameRate"];
        set => GraphicSettings["TargetFrameRate"] = value;
    }
    
    public static KeyCode Pos0Keycode {
        get => (KeyCode) GameSettings["POS_0"];
        set => GameSettings["POS_0"] = value;
    }
    public static KeyCode Pos1Keycode {
        get => (KeyCode) GameSettings["POS_1"];
        set => GameSettings["POS_1"] = value;
    }
    public static KeyCode Pos2Keycode {
        get => (KeyCode) GameSettings["POS_2"];
        set => GameSettings["POS_2"] = value;
    }
    public static KeyCode Pos3Keycode {
        get => (KeyCode) GameSettings["POS_3"];
        set => GameSettings["POS_3"] = value;
    }
    
    public static Languages CurrentLanguage {
        get => (Languages) GeneralSettings["CurrentLanguage"];
        set => GeneralSettings["CurrentLanguage"] = value;
    }

    public static Dictionary<string, Dictionary<string, object>> SettingValues;

    public static void ApplySettings() {
        Screen.SetResolution(ScreenResolution.x, ScreenResolution.y, FullScreenMode);
        Application.targetFrameRate = TargetFrameRate;
        Events.OnLanguageChange.Invoke();
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

        ApplySettings();
        Events.OnSettingsUpdate.Invoke();
    }
    
    public static void SaveSettings() {
        var path = Path.Combine(Constants.DataPath, "settings.json");
        try {
            File.WriteAllText(path, Json.Serialize(SettingValues, false));
            Debug.Log($"Saved settings to path {path}");
        } catch (Exception e) {
            Debug.LogWarning($"Cannot save settings to path {path}:\n{e}");
        }
    }

    private static void DecodeSettings(Dictionary<string, object> settings) {
        SettingValues = Json.Deserialize(Resources.Load<TextAsset>("defaultSettings").text).As<string, object, string, Dictionary<string, object>>();

        if (settings.ContainsKey("General")) {
            if (settings["General"] is Dictionary<string, object> general) {
                if (general.ContainsKey("CurrentLanguage")) CurrentLanguage = (Languages) general["CurrentLanguage"];
            }
        }
        
        if (settings.ContainsKey("Audio")) {
            if (settings["General"] is Dictionary<string, object> audio) { }
        }
        
        if (settings.ContainsKey("Graphic")) {
            if (settings["Graphic"] is Dictionary<string, object> graphic) {
                if (graphic.ContainsKey("ScreenResolution")) ScreenResolution = (Vector2Int) graphic["ScreenResolution"];
                if (graphic.ContainsKey("FullScreenMode")) FullScreenMode = (FullScreenMode) graphic["FullScreenMode"];
            }
        }
        
        if (settings.ContainsKey("Game")) {
            if (settings["Game"] is Dictionary<string, object> game) {
                if (game.ContainsKey("POS_0")) Pos0Keycode = (KeyCode) game["POS_0"];
                if (game.ContainsKey("POS_1")) Pos1Keycode = (KeyCode) game["POS_1"];
                if (game.ContainsKey("POS_2")) Pos2Keycode = (KeyCode) game["POS_2"];
                if (game.ContainsKey("POS_3")) Pos3Keycode = (KeyCode) game["POS_3"];
            }
        }
    }
}