using System;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StigmaStartup : MonoBehaviour {
    public string introScene;
    public void Startup() {
        Settings.LoadSettings();
        Settings.SaveSettings();
        Events.OnApplicationQuit.AddListener(Settings.SaveSettings);
        Events.OnLanguageChange.AddListener(SettingScreen.UpdateProps.Invoke);
    }

    private void Awake() {
        Startup();
    }

    private void Start() {
        GameManager.Instance.LoadScene(introScene, Trans.FadeEnd);
    }
}