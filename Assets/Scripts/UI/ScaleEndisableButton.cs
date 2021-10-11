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
            foreach (var target in targets) {
                var graphic = target.GetComponent<Graphic>();
                var graphics = target.GetComponentsInChildren<Graphic>();
                graphic.DOFade(0, 0);
                graphics.Do(g => g.DOFade(0, 0));
            }
            targets.Do(t => t.gameObject.SetActive(false));
        }

        public void OnPointerClick(PointerEventData eventData) {
            StopAllCoroutines();
            enableTargets = !enableTargets;
            StartCoroutine(Scale());
        }

        public IEnumerator Scale() {
            //var scale = enableTargets ? Vector2.one : Vector2.zero;
            int opacity = enableTargets ? 1 : 0;
            float delay = 0.3f;
            var realTargets = enableTargets ? targets : targets.Reverse().ToArray();
            string id = $"ScaleEndisable_{gameObject.GetInstanceID()}";
            DOTween.Kill(id, false);
            realTargets.Do(t => t.gameObject.SetActive(true));
            foreach (var target in realTargets) {
                var graphics = target.GetComponentsInChildren<Graphic>();
                graphics.Do(g => g.DOFade(opacity, delay).SetId(id));
                
                var graphic = target.GetComponent<Graphic>();
                graphic.DOFade(opacity, delay).SetId(id);
                
                if (!enableTargets) 
                    DOTween.Sequence().AppendCallback(() => target.gameObject.SetActive(false)).SetDelay(interval * realTargets.Length + duration).SetId(id);
                
                yield return new WaitForSecondsRealtime(interval);
            }
        }
    }
}