using Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    public class PauseButton : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            PlayManager.Instance.TogglePause();
        }
    }
}