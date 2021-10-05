using System;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;

namespace Core {
    public class JudgmentText : MonoBehaviour {
        public TMP_Text text;

        private void Start() {
            var duration = (float) (60 / PlayManager.Instance.currentBpm * 2);
            text.DOColor(new Color(text.color.r, text.color.g, text.color.b, 0), duration);
            transform.DOMoveY(this.transform.position.y - duration / 8, duration);
        }
    }
}