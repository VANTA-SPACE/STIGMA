using UnityEngine;
using UnityEngine.SceneManagement;

public class StigmaStartup : MonoBehaviour {
    public string introScene;
    public void Startup() {
        SceneManager.LoadScene(introScene);
    }

    private void Awake() {
        Startup();
    }
}