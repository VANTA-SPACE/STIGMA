using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DebugObj {
    public class SceneMover : MonoBehaviour {
        private void Awake() {
            if (GameObject.Find("GameManager") == null) SceneManager.LoadScene("SceneStartup");
        }
    }
}