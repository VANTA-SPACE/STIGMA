using System;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace UI {
    public class SettingApplyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        public bool applyOnly = false;
        public Graphic graphic;
        public float sizeMultiplier = 1.05f;
        public Color32 color = Color.white;

        private void Awake() {
            graphic.color = color;
        }

        public void OnPointerClick(PointerEventData eventData) {
            global::Settings.ApplySettings();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (applyOnly) return;
            graphic.DOColor(color, 0.1f);
            graphic.GetComponent<RectTransform>().DOScale(Vector3.one * sizeMultiplier, 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (applyOnly) return;
            graphic.DOColor(color.WithAlpha(192), 0.1f);
            graphic.GetComponent<RectTransform>().DOScale(Vector3.one, 0.1f);
        }
    }
}