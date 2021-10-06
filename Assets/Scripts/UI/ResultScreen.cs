using System;
using System.Collections.Generic;
using Core;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class ResultScreen : MonoBehaviour {
        private static ResultScreen _instance;
        
        public TMP_Text perfectText;
        public TMP_Text goodText;
        public TMP_Text badText;
        public TMP_Text missText;

        public TMP_Text fcText;

        public TMP_Text accText;
        public TMP_Text rankText;
        public TMP_Text scoreText;

        
        public static void ShowResultScene() {
            GameManager.Instance.LoadScene("ResultScene", Trans.FadeStart | Trans.FadeEnd);
        }

        private void Awake() {
            _instance = this;
            var manager = PlayManager.Instance;
            Init(manager.Accurary, manager.Totalnotes, manager.Score, manager.JudgmentCount);
            Show();
        }

        private void Init(float accurary, int totalnote, float score, Dictionary<Judgment, int> judgmentCount) {
            var totalMiss = judgmentCount[Judgment.Miss];
            accText.text = (accurary / totalnote).ToString("###.00") + "%";
            if (accurary / totalnote >= 100) {
                rankText.text = "<color=#4D45FF>Ϛ</color>";
            } else if (accurary / totalnote >= 97) {
                rankText.text = "<color=#ff9f1f>Σ</color>";
            } else if (accurary / totalnote >= 95) {
                rankText.text = "<color=#ffdb3f>S</color>";
            } else if (accurary / totalnote >= 90) {
                rankText.text = "<color=#7fff00>A</color>";
            } else if (accurary / totalnote >= 80) {
                rankText.text = "<color=#2fffaf>B</color>";
            } else if (accurary / totalnote >= 70) {
                rankText.text = "<color=#2f9fff>C</color>";
            } else {
                rankText.text = "<color=#ff3f1f>D</color>";
            }

            if (accurary / totalnote >= 100) {
                fcText.text = "<color=#ffdb3f>AP</color>";
            } else if (totalMiss == 0) {
                fcText.text = "<color=#2f9fff>FC</color>";
            } else {
                fcText.text = "";
            }
            
            perfectText.text = $"<size=36>(E{judgmentCount[Judgment.PerfectEarly]} / L{judgmentCount[Judgment.PerfectLate]})</size> {judgmentCount[Judgment.Perfect]}";
            goodText.text = $"<size=36>(E{judgmentCount[Judgment.GoodEarly]} / L{judgmentCount[Judgment.GoodLate]})</size> {judgmentCount[Judgment.Good]}";
            badText.text = $"{judgmentCount[Judgment.Bad]}";
            missText.text = $"{totalMiss}";
            scoreText.text = score.ToString("#,###,##0");
        }
        private void Show() {
            gameObject.SetActive(true);
        }
    }
}