using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace UI {
    public class PauseButton : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            PlayManager.Instance.TogglePause();
        }
    }
}