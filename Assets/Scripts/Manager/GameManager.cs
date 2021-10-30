using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

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

    public class GameManager : DontDestroyManager<GameManager> {
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

        public float Timescale;

        public static bool DoQuit;

        public List<KeyCode> invalidKeys = new List<KeyCode> {
            KeyCode.None
        };

        public List<KeyCode> mouseKeys = new List<KeyCode> {
            KeyCode.Mouse0,
            KeyCode.Mouse1,
            KeyCode.Mouse2,
            KeyCode.Mouse3,
            KeyCode.Mouse4,
            KeyCode.Mouse5,
            KeyCode.Mouse6
        };

        public float transitionLength = 0.3f;
        private bool _doingEffect = false;

        public static int ScreenWidth => Screen.width;
        public static int ScreenHeight => Screen.height;

        public override void Init() {
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
            panel1.gameObject.SetActive(false);
            panel2.gameObject.SetActive(false);
            panel3.gameObject.SetActive(false);
            panel4.gameObject.SetActive(false);
            panel5.gameObject.SetActive(false);
            _panel1Rect.sizeDelta = new Vector2(ScreenWidth, ScreenHeight);
            _panel2Rect.sizeDelta = new Vector2(ScreenWidth, ScreenHeight);
            _panel3Rect.sizeDelta = new Vector2(ScreenWidth, ScreenHeight);
            _panel4Rect.sizeDelta = new Vector2(ScreenWidth, ScreenHeight);
            _panel5Rect.sizeDelta = new Vector2(ScreenWidth, ScreenHeight);

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
            Debug.Log($"Darken {ScreenWidth} {ScreenHeight}");
            ResetPanel();
            DOTween.Sequence().AppendCallback(() => callback?.Invoke()).SetDelay(transitionLength * 2);
            bool flag = false;
            if (transitionType.HasFlag(Trans.FromUp)) {
                flag = true;
                panel1.color = Color.black;
                panel1.gameObject.SetActive(true);
                _panel1Rect.anchoredPosition = new Vector2(0, ScreenHeight);
                _panel1Rect.DOAnchorPosY(transitionType.HasFlag(Trans.FromDown) ? ScreenHeight / 2 : 0,
                    transitionLength);
            }

            if (transitionType.HasFlag(Trans.FromDown)) {
                flag = true;
                panel2.color = Color.black;
                panel2.gameObject.SetActive(true);
                _panel2Rect.anchoredPosition = new Vector2(0, -ScreenHeight);
                _panel2Rect.DOAnchorPosY(transitionType.HasFlag(Trans.FromUp) ? -ScreenHeight / 2 : 0,
                    transitionLength);
            }

            if (transitionType.HasFlag(Trans.FromLeft)) {
                flag = true;
                panel3.color = Color.black;
                panel3.gameObject.SetActive(true);
                _panel3Rect.anchoredPosition = new Vector2(-ScreenWidth, 0);
                _panel3Rect.DOAnchorPosX(transitionType.HasFlag(Trans.FromRight) ? -ScreenWidth / 2 : 0,
                    transitionLength);
            }

            if (transitionType.HasFlag(Trans.FromRight)) {
                flag = true;
                panel4.color = Color.black;
                panel4.gameObject.SetActive(true);
                _panel4Rect.anchoredPosition = new Vector2(ScreenWidth, 0);
                _panel4Rect.DOAnchorPosX(transitionType.HasFlag(Trans.FromLeft) ? ScreenWidth / 2 : 0,
                    transitionLength);
            }

            if (transitionType.HasFlag(Trans.FadeStart)) {
                flag = true;
                panel5.color = Color.clear;
                panel5.gameObject.SetActive(true);
                panel5.DOColor(Color.black, transitionLength);
            }

            if (!flag) {
                panel5.color = Color.blue;
            }
        }

        private static void SetAnchorPosX(RectTransform transform, float x) {
            transform.anchoredPosition = new Vector2(x, transform.anchoredPosition.y);
        }

        private static void SetAnchorPosY(RectTransform transform, float y) {
            transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, y);
        }

        public void Undarken(Trans transitionType) {
            ResetPanel(false);
            Debug.Log($"Undarken {ScreenWidth} {ScreenHeight}");
            StartCoroutine(StigmaUtils.SetDelay(() => {
                _doingEffect = false;
                ResetPanel();
            }, transitionLength));
            if (transitionType.HasFlag(Trans.ToUp)) {
                panel1.color = Color.black;
                SetAnchorPosY(_panel1Rect, transitionType.HasFlag(Trans.ToDown) ? ScreenHeight / 2 : 0);
                panel1.gameObject.SetActive(true);
                _panel1Rect.DOAnchorPosY(ScreenHeight, transitionLength);
            }

            if (transitionType.HasFlag(Trans.ToDown)) {
                panel2.color = Color.black;
                SetAnchorPosY(_panel2Rect, transitionType.HasFlag(Trans.ToDown) ? -ScreenHeight / 2 : 0);
                panel2.gameObject.SetActive(true);
                _panel2Rect.DOAnchorPosY(-ScreenHeight, transitionLength);
            }

            if (transitionType.HasFlag(Trans.ToLeft)) {
                panel3.color = Color.black;
                SetAnchorPosX(_panel3Rect, transitionType.HasFlag(Trans.ToRight) ? -ScreenWidth / 2 : 0);
                panel3.gameObject.SetActive(true);
                Debug.Log("To Left");
                _panel3Rect.DOAnchorPosX(-ScreenWidth, transitionLength);
            }

            if (transitionType.HasFlag(Trans.ToRight)) {
                panel4.color = Color.black;
                SetAnchorPosX(_panel4Rect, transitionType.HasFlag(Trans.ToLeft) ? ScreenWidth / 2 : 0);
                panel4.gameObject.SetActive(true);
                Debug.Log("To Right");
                _panel4Rect.DOAnchorPosX(ScreenWidth, transitionLength);
            }

            if (transitionType.HasFlag(Trans.FadeEnd)) {
                panel5.color = Color.black;
                panel5.gameObject.SetActive(true);
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

        public void LoadScene(string sceneToLoad, Trans transitionType = Trans.FromRight | Trans.ToLeft,
            bool resetTimescale = true, LoadSceneMode mode = LoadSceneMode.Single) {
            Time.timeScale = 1;
            DOTween.timeScale = 1;
            Transition(transitionType, () => SceneManager.LoadScene(sceneToLoad, mode));
        }

        public bool ValidAnyKeyDown(bool checkMouseKey = true) {
            if (!Input.anyKey) return false;
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) {
                if (invalidKeys.Contains(key)) continue;
                if (!checkMouseKey && mouseKeys.Contains(key)) continue;
                if (!Input.GetKeyDown(key)) continue;
                return true;
            }

            return false;
        }

        public bool ValidAnyKey(bool checkMouseKey = true) {
            if (!Input.anyKey) return false;
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) {
                if (invalidKeys.Contains(key)) continue;
                if (!checkMouseKey && mouseKeys.Contains(key)) continue;
                if (!Input.GetKey(key)) continue;
                return true;
            }

            return false;
        }

        private void OnApplicationQuit() {
            Events.OnApplicationQuit.Invoke();
        }

        private void Update() {
            Timescale = Time.timeScale;
        }
    }
}