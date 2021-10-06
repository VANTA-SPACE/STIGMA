using UnityEngine;
using UnityEngine.SceneManagement;

public class StigmaStartup : MonoBehaviour {
    public string introScene;
    public void Startup() {
        Settings.LoadSettings();
        Settings.SaveSettings();
        SceneManager.LoadScene(introScene);
    }

    private void Awake() {
        Startup();
    }
}