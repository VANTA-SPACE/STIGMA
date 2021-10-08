using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Level;
using FMOD;
using Locale;
using Manager;
using Serialization;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using Coroutine = Utils.Coroutine;
using Debug = UnityEngine.Debug;
using Thread = System.Threading.Thread;

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

    public static Resolution ScreenResolution {
        get => GraphicSettings["ScreenResolution"].As(new Resolution() {width = 1920, height = 1080, refreshRate = 60});
        set => GraphicSettings["ScreenResolution"] = value;
    }

    public static FullScreenMode FullScreenMode {
        get => GraphicSettings["FullScreenMode"].As(FullScreenMode.FullScreenWindow);
        set => GraphicSettings["FullScreenMode"] = value;
    }

    public static int TargetFrameRate {
        get => GeneralSettings["TargetFrameRate"].As(60);
        set => GeneralSettings["TargetFrameRate"] = value;
    }

    public static bool ShowELOnPerfect {
        get => GameSettings["ShowELOnPerfect"].As(true);
        set => GameSettings["ShowELOnPerfect"] = value;
    }

    public static int MasterVolume {
        get => AudioSettings["MasterVolume"].As(100);
        set => AudioSettings["MasterVolume"] = value;
    }



    public static KeyCode Pos0Keycode {
        get => ControlSettings["POS_0"].As(KeyCode.D);
        set => ControlSettings["POS_0"] = value;
    }

    public static KeyCode Pos1Keycode {
        get => ControlSettings["POS_1"].As(KeyCode.F);
        set => ControlSettings["POS_1"] = value;
    }

    public static KeyCode Pos2Keycode {
        get => ControlSettings["POS_2"].As(KeyCode.J);
        set => ControlSettings["POS_2"] = value;
    }

    public static KeyCode Pos3Keycode {
        get => ControlSettings["POS_3"].As(KeyCode.K);
        set => ControlSettings["POS_3"] = value;
    }

    public static Language CurrentLanguage {
        get => (Language) GeneralSettings.GetOrDefault("CurrentLanguage");
        set => GeneralSettings["CurrentLanguage"] = value;
    }

    public static Dictionary<string, Dictionary<string, object>> SettingValuesTemp { get; private set; }
    public static Dictionary<string, Dictionary<string, object>> SettingValues { get; private set; }
    public static Dictionary<string, Dictionary<string, object>> SettingData { get; private set; }

    public static void ApplySettings() {
        GeneralSettings = SettingValuesTemp["General"].Copy();
        AudioSettings = SettingValuesTemp["Audio"].Copy();
        GraphicSettings = SettingValuesTemp["Graphic"].Copy();
        GameSettings = SettingValuesTemp["Game"].Copy();
        ControlSettings = SettingValuesTemp["Control"].Copy();
        Screen.SetResolution(ScreenResolution.width, ScreenResolution.height, FullScreenMode, ScreenResolution.refreshRate);
        Application.targetFrameRate = TargetFrameRate;
        Events.OnLanguageChange.Invoke();
        if (SoundManager.Instance != null) SoundManager.Instance.SetVolume(MasterVolume / 100f);
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
        } catch (Exception e) {
            DecodeSettings(new Dictionary<string, object>());
        }

        SettingValuesTemp = new Dictionary<string, Dictionary<string, object>>();
        SettingValuesTemp["General"] = GeneralSettings.Copy();
        SettingValuesTemp["Audio"] = AudioSettings.Copy();
        SettingValuesTemp["Graphic"] = GraphicSettings.Copy();
        SettingValuesTemp["Game"] = GameSettings.Copy();
        SettingValuesTemp["Control"] = ControlSettings.Copy();
        ApplySettings();
        Events.OnSettingsUpdate.Invoke();
    }

    public static void SaveSettings() {
        var path = Path.Combine(Constants.DataPath, "settings.json");
        try {
            File.WriteAllText(path, Json.Serialize(SettingValues, false));
        } catch (Exception e) { }
    }

    public static IEnumerator LoadSettingsCo() {
        var path = Path.Combine(Constants.DataPath, "settings.json");
        var exists = false;
        try {
            exists = File.Exists(path);
        } catch { }

        if (!exists) {
            InitalizeSettings();
        } else {
            var data = Json.Deserialize(File.ReadAllText(path));
            yield return DecodeSettingsCo((Dictionary<string, object>) data);
        }

        SettingValuesTemp = SettingValues.Copy();
        ApplySettings();
        Events.OnSettingsUpdate.Invoke();
        yield break;
    }

    private static Dictionary<TKey, TValue> Merge<TKey, TValue>(Dictionary<TKey, TValue> d1,
        Dictionary<TKey, TValue> d2) {
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

    private static IEnumerator MergeCo<TKey, TValue>(Dictionary<TKey, TValue> d1, Dictionary<TKey, TValue> d2,
        CoroutineResult<Dictionary<TKey, TValue>> coroutineResult) {
        try {
            if (d1 == null) {
                coroutineResult.ReturnValue(d2);
                yield break;
            }

            if (d2 == null) {
                coroutineResult.ReturnValue(d1);
                yield break;
            }

            var result = new Dictionary<TKey, TValue>();
            foreach (var (key, value) in d1) {
                result[key] = value;
            }

            foreach (var (key, value) in d2) {
                if (result.ContainsKey(key) && value == null) continue;
                result[key] = value;
            }

            coroutineResult.ReturnValue(result);
        } catch {
            coroutineResult.ReturnValue(d1);
        }
    }

    private static void DecodeSettings(Dictionary<string, object> settings) {
        InitalizeSettings();
        GeneralSettings = Merge(SettingValues["General"],
            settings.GetOrDefault("General") as Dictionary<string, object>);
        AudioSettings = Merge(SettingValues["Audio"], settings.GetOrDefault("Audio") as Dictionary<string, object>);
        GraphicSettings = Merge(SettingValues["Graphic"],
            settings.GetOrDefault("Graphic") as Dictionary<string, object>);
        GameSettings = Merge(SettingValues["Game"], settings.GetOrDefault("Game") as Dictionary<string, object>);
        ControlSettings = Merge(SettingValues["Control"],
            settings.GetOrDefault("Control") as Dictionary<string, object>);
    }

    private static void InitalizeSettings() {
        SettingValues = Json.Deserialize(Resources.Load<TextAsset>("defaultSettings").text)
            .As<string, object, string, Dictionary<string, object>>();
        SettingData = Json.Deserialize(Resources.Load<TextAsset>("settingProperties").text)
            .As<string, object, string, Dictionary<string, object>>();
    }

    private static IEnumerator DecodeSettingsCo(Dictionary<string, object> settings) {
        InitalizeSettings();
        var r = new CoroutineResult<Dictionary<string, object>>();
        yield return MergeCo(SettingValues["General"], settings.GetOrDefault("General") as Dictionary<string, object>, r);
        GeneralSettings = r.Value;
        yield return MergeCo(SettingValues["Audio"], settings.GetOrDefault("Audio") as Dictionary<string, object>, r);
        AudioSettings = r.Value;
        yield return MergeCo(SettingValues["Graphic"], settings.GetOrDefault("Graphic") as Dictionary<string, object>, r);
        GraphicSettings = r.Value;
        yield return MergeCo(SettingValues["Game"], settings.GetOrDefault("Game") as Dictionary<string, object>, r);
        GameSettings = r.Value;
        yield return MergeCo(SettingValues["Control"], settings.GetOrDefault("Control") as Dictionary<string, object>, r);
        ControlSettings = r.Value;
    }
}