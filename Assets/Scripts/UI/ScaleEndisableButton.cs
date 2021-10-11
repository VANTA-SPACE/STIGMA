using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace UI {
    public class ScaleEndisableButton : MonoBehaviour, IPointerClickHandler {
        public bool enableTargets;
        public RectTransform[] targets = { };
        public float interval = 0.1f;
        public float duration = 0.3f;

        private void Awake() {
            StartCoroutine(Scale(true));
        }

        public void OnPointerClick(PointerEventData eventData) {
            StopAllCoroutines();
            StartCoroutine(Scale(true));
            enableTargets = !enableTargets;
            StartCoroutine(new List<object> {
                null,
                Scale(false)
            }.GetEnumerator());
        }

        public IEnumerator Scale(bool immediate = false) {
            //var scale = enableTargets ? Vector2.one : Vector2.zero;
            int opacity = enableTargets ? 1 : 0;
            float delay = immediate ? 0 : 0.3f;
            var realTargets = enableTargets ? targets : targets.Reverse();
            string id = $"ScaleEndisable_{gameObject.GetInstanceID()}";
            DOTween.Kill(id, true);
            foreach (var target in realTargets) {
                if (enableTargets) target.gameObject.SetActive(true);
                var graphic = target.GetComponent<Graphic>();
                var graphics = target.GetComponentsInChildren<Graphic>();
                graphic.DOFade(opacity, delay).SetId(id);
                graphics.Do(g => g.DOFade(opacity, delay).SetId(id));
                if (!enableTargets)
                    DOTween.Sequence().AppendCallback(() => target.gameObject.SetActive(false)).SetDelay(delay);
                //target.DOScale(scale, duration).SetId(id);
                if (!immediate) yield return new WaitForSecondsRealtime(interval);
            }
        }
    }
}