using System;
using System.Collections;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StigmaStartup : MonoBehaviour {
    public string introScene;
    public static bool StartedUp { get; private set; }

    public Image logo;
    public TMP_Text vantaGames;
    
    public static IEnumerator StartupCo() {
        yield return Settings.LoadSettingsCo();
        Events.OnApplicationQuit.AddListener(Settings.SaveSettings);
        Events.OnLanguageChange.AddListener(SettingScreen.UpdateProps.Invoke);
        StartedUp = true;
        Debug.Log("Started up");
    }

    public IEnumerator ShowLogo() {
        yield return new WaitForSeconds(0.5f);
        logo.DOColor(Color.white, 1f);
        yield return new WaitForSeconds(1f);
        vantaGames.DOColor(Color.white, 1f);
        yield return new WaitForSeconds(1);
        while (!StartedUp) {
            Debug.Log("Not started up");
            yield return null;
        }
        GameManager.Instance.LoadScene(introScene, Trans.FadeEnd);
    }
    
    private void Awake() {
        StartCoroutine(StartupCo());
    }

    private void Start() {
        StartCoroutine(ShowLogo());
    }
}