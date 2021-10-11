using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace UI {
    public class StartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        public Graphic graphic;
        public float sizeMultiplier = 1.05f;

        public bool interactable {
            get => _interactable;
            set {
                if (_interactable == value) return;
                if (value) {
                    graphic.DOColor( new Color32(0, 0, 0, 0), 0.1f);
                } else {
                    graphic.DOColor( new Color32(0, 0, 0, 96), 0.1f);
                }
                _interactable = value;
            }
        }

        private bool _interactable;

        private void Awake() {
            graphic.color = new Color32(0, 0, 0, 96);
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (!interactable) return;
            LevelSelectManager.Instance.StartLevel();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!interactable) return;
            var rt = GetComponent<RectTransform>();
            rt.DOScale(Vector3.one * sizeMultiplier, 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            var rt = GetComponent<RectTransform>();
            rt.DOScale(Vector3.one, 0.1f);
        }
    }
}