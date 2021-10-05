using System;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UI {
    public class PauseMenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        public TMP_Text text;
        public ButtonType buttonType;
        public enum ButtonType {
            Resume,
            Retry,
            Exit
        }

        private void Awake() {
            text ??= GetComponent<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData) {
            switch (buttonType) {
                case ButtonType.Resume:
                    PlayManager.Instance.UnpauseGame();
                    break;
                case ButtonType.Retry:
                    PlayManager.Instance.UnpauseGame();
                    PlayManager.Instance.StartPlay();
                    break;
                case ButtonType.Exit:
                    var transition = Trans.FromLeft | Trans.FromRight | Trans.ToUp | Trans.ToDown;
                    GameManager.Instance.LoadScene(Constants.INTRO_SCENE, transition);
                    break;
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            text.DOColor(Color.white, 0.1f);
            text.GetComponent<RectTransform>().DOScale(Vector3.one * 1.1f, 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            text.DOColor(new Color32(255, 255, 255, 192), 0.1f);
            text.GetComponent<RectTransform>().DOScale(Vector3.one, 0.1f);
        }
    }
}