using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class Gauge : MonoBehaviour {
        public RectTransform rectTransform;
        public RectTransform gaugeLine;
        public Image image;
        public bool playing;
        public double currRat;
        public double currValue;

        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
            gaugeLine.anchoredPosition = new Vector2(Constants.GAUGE_RESULT_F / 100 * 450, 0);
            image = GetComponent<Image>();
            gameObject.SetActive(false);
        }

        public void StartGauge() {
            playing = true;
            rectTransform.sizeDelta = Vector2.zero;
            currRat = 0;
            gameObject.SetActive(true);
        }
        
        public void StopGauge() {
            playing = false;
            gameObject.SetActive(false);
        }

        void Update() {
            if (!playing) return;
            var value = PlayManager.Instance.GaugeValue;
            var rat = value * 450 / 100;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (rat == currRat) return;
            DOTween.Kill("ChangeGauge", false);
            if (value <= Constants.GAUGE_RESULT_F && currValue > Constants.GAUGE_RESULT_F) {
                image.DOColor(new Color32(192, 0, 0, 255), 0.3f).SetId("ChangeGauge");
            } else if (value > Constants.GAUGE_RESULT_F && currValue <= Constants.GAUGE_RESULT_F) {
                image.DOColor(Color.white, 0.3f).SetId("ChangeGauge");
            }
            rectTransform.DOSizeDelta(new Vector2(rat, 20), 0.3f).SetId("ChangeGauge");
            currRat = rat;
            currValue = value;
        }
    }
}