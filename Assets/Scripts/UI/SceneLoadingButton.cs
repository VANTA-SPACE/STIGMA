using System;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI {
    public class SceneLoadingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        public TMP_Text text;
        public Trans transitions;
        public string sceneToLoad;
        public float sizeMultiplier = 1.1f;
        public Color32 color = Color.white;

        private void Awake() {
            text.color = color;
        }

        private void OnMouseDown() {
            GameManager.Instance.LoadScene(sceneToLoad, transitions);
        }

        public void OnPointerClick(PointerEventData eventData) {
            GameManager.Instance.LoadScene(sceneToLoad, transitions);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            text.DOColor(color, 0.1f);
            text.GetComponent<RectTransform>().DOScale(Vector3.one * sizeMultiplier, 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            text.DOColor(color.WithAlpha(192), 0.1f);
            text.GetComponent<RectTransform>().DOScale(Vector3.one, 0.1f);
        }
    }
}