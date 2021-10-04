using System;
using System.Collections;
using System.Collections.Generic;
using Core.Level;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Manager {
    [Flags]
    public enum Trans {
        Unset = 0,
        FromUp = 0b1,
        FromDown = 0b10,
        FromLeft = 0b100,
        FromRight = 0b1000,
        FadeStart = 0b10000,
        ToUp = 0b100000,
        ToDown = 0b1000000,
        ToLeft = 0b10000000,
        ToRight = 0b100000000,
        FadeEnd = 0b1000000000
    }

    public class GameManager : MonoBehaviour {
        public Image panel1;
        public Image panel2;
        public Image panel3;
        public Image panel4;
        public Image panel5;
        private RectTransform _panel1Rect;
        private RectTransform _panel2Rect;
        private RectTransform _panel3Rect;
        private RectTransform _panel4Rect;
        private RectTransform _panel5Rect;
    
    
        public float transitionLength = 3;
        private bool _doingEffect = false;
    
        public static GameManager Instance => _instance;
        private static GameManager _instance;

        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            _instance = this;
            _panel1Rect = panel1.GetComponent<RectTransform>();
            _panel2Rect = panel2.GetComponent<RectTransform>();
            _panel3Rect = panel3.GetComponent<RectTransform>();
            _panel4Rect = panel4.GetComponent<RectTransform>();
            _panel5Rect = panel5.GetComponent<RectTransform>();
        }

        private void ResetPanel(bool resetPosition = true) {
            panel1.color = Color.clear;
            panel2.color = Color.clear;
            panel3.color = Color.clear;
            panel4.color = Color.clear;
            panel5.color = Color.clear;

            if (resetPosition) {
                _panel1Rect.anchoredPosition = new Vector2(0, 0);
                _panel2Rect.anchoredPosition = new Vector2(0, 0);
                _panel3Rect.anchoredPosition = new Vector2(0, 0);
                _panel4Rect.anchoredPosition = new Vector2(0, 0);
            }
            _panel5Rect.anchoredPosition = new Vector2(0, 0);
        }

        public void Darken(Trans transitionType, Action callback = null) {
            if (_doingEffect) return;
            _doingEffect = true;
            Debug.Log("Darken");
            ResetPanel();
            DOTween.Sequence().AppendCallback(() => callback?.Invoke()).SetDelay(transitionLength * 2);
            bool flag = false;
            if (transitionType.HasFlag(Trans.FromUp)) {
                flag = true;
                panel1.color = Color.black;
                _panel1Rect.anchoredPosition = new Vector2(0, 1080);
                _panel1Rect.DOAnchorPosY(transitionType.HasFlag(Trans.FromDown) ? 540 : 0, transitionLength);
            }
            if (transitionType.HasFlag(Trans.FromDown)) {
                flag = true;
                panel2.color = Color.black;
                _panel2Rect.anchoredPosition = new Vector2(0, -1080);
                _panel2Rect.DOAnchorPosY(transitionType.HasFlag(Trans.FromUp) ? -540 : 0, transitionLength);
            }
            if (transitionType.HasFlag(Trans.FromLeft)) {
                flag = true;
                panel3.color = Color.black;
                _panel3Rect.anchoredPosition = new Vector2(-1920, 0);
                _panel3Rect.DOAnchorPosX(transitionType.HasFlag(Trans.FromRight) ? -960 : 0, transitionLength);
            }
            if (transitionType.HasFlag(Trans.FromRight)) {
                flag = true;
                panel4.color = Color.black;
                _panel4Rect.anchoredPosition = new Vector2(1920, 0);
                _panel4Rect.DOAnchorPosX(transitionType.HasFlag(Trans.FromLeft) ? 960 : 0, transitionLength);
            }
            if (transitionType.HasFlag(Trans.FadeStart)) {
                flag = true;
                panel5.color = Color.clear;
                panel5.DOColor(Color.black, transitionLength);
            }

            if (!flag) {
                panel5.color = Color.blue;
            }
        }

        public void Undarken(Trans transitionType) {
            ResetPanel(false);
            Debug.Log("Undarken");
            DOTween.Sequence().AppendCallback(() => _doingEffect = false).SetDelay(transitionLength);
            if (transitionType.HasFlag(Trans.ToUp)) {
                panel1.color = Color.black;
                _panel1Rect.DOAnchorPosY(1080, transitionLength);
            }
            if (transitionType.HasFlag(Trans.ToDown)) {
                panel2.color = Color.black;
                _panel2Rect.DOAnchorPosY(-1080, transitionLength);
            }
            if (transitionType.HasFlag(Trans.ToLeft)) {
                panel3.color = Color.black;
                _panel3Rect.DOAnchorPosX(-1920, transitionLength);
            }
            if (transitionType.HasFlag(Trans.ToRight)) {
                panel4.color = Color.black;
                _panel4Rect.DOAnchorPosX(1920, transitionLength);
            }
            if (transitionType.HasFlag(Trans.FadeEnd)) {
                panel5.color = Color.black;
                panel5.DOColor(Color.clear, transitionLength);
            }
        }

        public void Transition(Trans transitionType, Action callback = null) {
            if (_doingEffect) return;
            Darken(transitionType, () => {
                callback?.Invoke();
                Undarken(transitionType);
            });
        }

        public void LoadScene(string sceneToLoad, Trans transitionType = Trans.FromRight | Trans.ToLeft) {
            Transition(transitionType, () => SceneManager.LoadScene(sceneToLoad));
        }
    }
}