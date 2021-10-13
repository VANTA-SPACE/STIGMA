using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class CanvasCameraSetter : MonoBehaviour {
        public Canvas canvas;

        private void Awake() {
            canvas ??= GetComponent<Canvas>();
        }

        private void Update() {
            if (canvas.worldCamera == null) 
                canvas.worldCamera = Camera.current;
        }
    }
}