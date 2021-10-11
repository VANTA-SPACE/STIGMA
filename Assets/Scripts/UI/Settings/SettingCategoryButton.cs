using System;
using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace UI.Settings {
    public class SettingCategoryButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        public SettingScreen screen;
        public Graphic[] graphics;
        public string categoryName;
        public float sizeMultiplier = 1.05f;
        [NonSerialized] public Color32 Color = UnityEngine.Color.white;

        private void Awake() {
            if (screen.CurrentCategory == categoryName) {
                Color = UnityEngine.Color.white;
            } else {
                Color = new Color32(255, 255, 255, 192);
            }
            foreach (var graphic in graphics) {
                graphic.color = Color;
            }
            screen.OnUpdateCategory.AddListener(() => {
                if (screen.CurrentCategory == categoryName) {
                    Color = UnityEngine.Color.white;
                } else {
                    Color = new Color32(255, 255, 255, 192);
                }
                foreach (var graphic in graphics) {
                    graphic.DOColor(Color, 0.1f);
                }
            });
        }

        public void OnPointerClick(PointerEventData eventData) {
            screen.CurrentCategory = categoryName;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            foreach (var graphic in graphics) {
                graphic.GetComponent<RectTransform>().DOScale(Vector3.one * sizeMultiplier, 0.1f);
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            foreach (var graphic in graphics) {
                graphic.GetComponent<RectTransform>().DOScale(Vector3.one, 0.1f);
            }
        }
    }
}