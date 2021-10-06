using System;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    public class SceneLoadingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        public TMP_Text text;
        public Trans transitions;
        public string sceneToLoad;

        private void OnMouseDown() {
            GameManager.Instance.LoadScene(sceneToLoad, transitions);
        }

        public void OnPointerClick(PointerEventData eventData) {
            GameManager.Instance.LoadScene(sceneToLoad, transitions);
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