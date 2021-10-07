using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private set => SettingValues["General"] = value;
    }
    public static Dictionary<string, object> AudioSettings {
        get {
            if (!SettingValues.ContainsKey("Audio")) {
                SettingValues["Audio"] = new Dictionary<string, object>();
            }

            return SettingValues["Audio"];
        }
        private set => SettingValues["Audio"] = value;
    }
    public static Dictionary<string, object> GraphicSettings {
        get {
            if (!SettingValues.ContainsKey("Graphic")) {
                SettingValues["Graphic"] = new Dictionary<string, object>();
            }

            return SettingValues["Graphic"];
        }
        private set => SettingValues["Graphic"] = value;
    }
    
    public static Dictionary<string, object> GameSettings {
        get {
            if (!SettingValues.ContainsKey("Game")) {
                SettingValues["Game"] = new Dictionary<string, object>();
            }

            return SettingValues["Game"];
        }
        private set => SettingValues["Game"] = value;
    }    
    public static Dictionary<string, object> ControlSettings {
        get {
            if (!SettingValues.ContainsKey("Control")) {
                SettingValues["Control"] = new Dictionary<string, object>();
            }

            return SettingValues["Control"];
        }
        private set => SettingValues["Control"] = value;
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
        get => (int) GeneralSettings["TargetFrameRate"];
        set => GeneralSettings["TargetFrameRate"] = value;
    }
    
    public static KeyCode Pos0Keycode {
        get => (KeyCode) ControlSettings["POS_0"];
        set => ControlSettings["POS_0"] = value;
    }
    public static KeyCode Pos1Keycode {
        get => (KeyCode) ControlSettings["POS_1"];
        set => ControlSettings["POS_1"] = value;
    }
    public static KeyCode Pos2Keycode {
        get => (KeyCode) ControlSettings["POS_2"];
        set => ControlSettings["POS_2"] = value;
    }
    public static KeyCode Pos3Keycode {
        get => (KeyCode) ControlSettings["POS_3"];
        set => ControlSettings["POS_3"] = value;
    }
    
    public static Language CurrentLanguage {
        get => (Language) GeneralSettings.GetOrDefault("CurrentLanguage");
        set => GeneralSettings["CurrentLanguage"] = value;
    }

    public static Dictionary<string, Dictionary<string, object>> SettingValues { get; private set; }
    public static Dictionary<string, Dictionary<string, object>> SettingData { get; private set; }

    public static void ApplySettings() {
        Screen.SetResolution(ScreenResolution.x, ScreenResolution.y, FullScreenMode);
        Application.targetFrameRate = TargetFrameRate;
        Events.OnLanguageChange.Invoke();
        SaveSettings();
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

    private static Dictionary<TKey, TValue> Merge<TKey, TValue>(Dictionary<TKey, TValue> d1, Dictionary<TKey, TValue> d2) {
        if (d1 == null) return d2;
        if (d2 == null) return d1;
        var result = new Dictionary<TKey, TValue>();
        foreach (var (key, value) in d1) {
            result[key] = value;
        }

        foreach (var (key, value) in d2) {
            if (result.ContainsKey(key) && value == null) continue;
            result[key] = value;
        }

        return result;
    }

    private static void DecodeSettings(Dictionary<string, object> settings) {
        SettingValues = Json.Deserialize(Resources.Load<TextAsset>("defaultSettings").text).As<string, object, string, Dictionary<string, object>>();
        SettingData = Json.Deserialize(Resources.Load<TextAsset>("settingProperties").text).As<string, object, string, Dictionary<string, object>>();

        GeneralSettings = Merge(SettingValues["General"], settings.GetOrDefault("General") as Dictionary<string, object>);
        AudioSettings =  Merge(SettingValues["Audio"], settings.GetOrDefault("Audio") as Dictionary<string, object>);
        GraphicSettings = Merge(SettingValues["Graphic"], settings.GetOrDefault("Graphic") as Dictionary<string, object>);
        GameSettings = Merge(SettingValues["Game"], settings.GetOrDefault("Game") as Dictionary<string, object>);
        ControlSettings = Merge(SettingValues["Control"], settings.GetOrDefault("Control") as Dictionary<string, object>);
        
        return;
        if (settings.ContainsKey("General")) {
            if (settings["General"] is Dictionary<string, object> general) {
                if (general.ContainsKey("CurrentLanguage")) CurrentLanguage = (Language) general["CurrentLanguage"];
                if (general.ContainsKey("TargetFrameRate")) TargetFrameRate = (int) general["TargetFrameRate"];
            }
        }
        
        if (settings.ContainsKey("Audio")) {
            if (settings["Audio"] is Dictionary<string, object> audio) { }
        }
        
        if (settings.ContainsKey("Graphic")) {
            if (settings["Graphic"] is Dictionary<string, object> graphic) {
                if (graphic.ContainsKey("ScreenResolution")) ScreenResolution = (Vector2Int) graphic["ScreenResolution"];
                if (graphic.ContainsKey("FullScreenMode")) FullScreenMode = (FullScreenMode) graphic["FullScreenMode"];
            }
        }
        
        if (settings.ContainsKey("Game")) {
            if (settings["Game"] is Dictionary<string, object> game) { }
        }
        
        if (settings.ContainsKey("Control")) {
            if (settings["Control"] is Dictionary<string, object> control) {
                if (control.ContainsKey("POS_0")) Pos0Keycode = (KeyCode) control["POS_0"];
                if (control.ContainsKey("POS_1")) Pos1Keycode = (KeyCode) control["POS_1"];
                if (control.ContainsKey("POS_2")) Pos2Keycode = (KeyCode) control["POS_2"];
                if (control.ContainsKey("POS_3")) Pos3Keycode = (KeyCode) control["POS_3"];
            }
        }
    }
}