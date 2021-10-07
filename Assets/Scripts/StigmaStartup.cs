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
    }

    private void Awake() {
        Startup();
    }

    private void Start() {
        GameManager.Instance.LoadScene(introScene, Trans.FadeEnd);
    }
}