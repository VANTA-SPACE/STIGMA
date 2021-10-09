using Manager;
using UnityEngine;

namespace UI {
    public class ProgressBar : MonoBehaviour {
        public RectTransform rectTransform;
        public bool playing;

        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        public void StartProgress() {
            playing = true;
            rectTransform.sizeDelta = Vector2.zero;
            gameObject.SetActive(true);
        }
        
        public void StopProgress() {
            playing = false;
            gameObject.SetActive(false);
        }

        void Update() {
            if (!playing) return;

            var rat = PlayManager.Instance.CurrentMilisec / PlayManager.Instance.LevelData.EndMSAnd4Beats;
            rectTransform.sizeDelta = new Vector2((float) (rat * 1920), 5);
        }
    }
}