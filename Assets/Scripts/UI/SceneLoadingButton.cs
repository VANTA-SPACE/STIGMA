using System;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace UI {
    public class SceneLoadingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        [FormerlySerializedAs("text")] 
        public Graphic graphic;
        public Trans transitions;
        public string sceneToLoad;
        public float sizeMultiplier = 1.05f;
        public Color32 color = Color.white;

        private void Awake() {
            graphic.color = color.WithAlpha((byte) (color.a * 192 / 255));
        }

        private void OnMouseDown() {
            GameManager.Instance.LoadScene(sceneToLoad, transitions);
        }

        public void OnPointerClick(PointerEventData eventData) {
            GameManager.Instance.LoadScene(sceneToLoad, transitions);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            graphic.DOColor(color, 0.1f);
            var rt = graphic.GetComponent<RectTransform>();
            rt.DOScale(Vector3.one * sizeMultiplier, 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            graphic.DOColor(color.WithAlpha((byte) (color.a * 192 / 255)), 0.1f);
            var rt = graphic.GetComponent<RectTransform>();
            rt.DOScale(Vector3.one, 0.1f);
        }
    }
}