using System;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Coroutine = Utils.Coroutine;

namespace UI {
    public class RadioButton : MonoBehaviour, IPointerClickHandler {
        private static Dictionary<string, int> Values => RadioButtonManager.Instance.Values;
        private static Dictionary<string, UnityEvent> TargetGraphics => RadioButtonManager.Instance.TargetGraphics;

        public bool enumFlag;
        public string id;
        public int value;
        public bool isDefault;
        public List<SerializableKeyValuePair<Graphic, SerializableKeyValuePair<Color, Color>>> graphics;
        public UnityEvent<bool> onUpdate = new UnityEvent<bool>();

        private void Awake() {
            RadioButtonManager.Instance.Values ??= new Dictionary<string, int>();
            RadioButtonManager.Instance.TargetGraphics ??= new Dictionary<string, UnityEvent>();
            if (!Values.ContainsKey(id)) Values[id] = 0;
            if (!TargetGraphics.ContainsKey(id)) TargetGraphics[id] = new UnityEvent();
            
            if (!isDefault) return;
            if (enumFlag) {
                Values[id] ^= value;
            } else {
                Values[id] = value;
            }
            
            StartCoroutine(Coroutine.MakeCoroutine(() => {
                UpdateGraphics(true);
            }));
        }

        private void Start() {
            if (!TargetGraphics.ContainsKey(id)) TargetGraphics[id] = new UnityEvent();
            TargetGraphics[id].AddListener(() => this?.UpdateGraphics(false));
        }

        public void OnPointerClick(PointerEventData eventData) {
            Debug.Log("Pointer Click");
            
            if (enumFlag) {
                Values[id] ^= value;
            } else {
                Values[id] = value;
            }
            TargetGraphics[id].Invoke();
        }

        public void UpdateGraphics(bool immediate = false) {
            var flag = enumFlag ? StigmaUtils.HasFlag(Values[id], value) : Values[id] == value;
            if (flag) {
                foreach (var (graphic, (_, color2)) in graphics) {
                    if (graphic == null) continue;
                    graphic.DOColor(color2, immediate ? 0 : 0.15f);
                }
                onUpdate.Invoke(true);
            } else {
                foreach (var (graphic, (color1, _)) in graphics) {
                    if (graphic == null) continue;
                    graphic.DOColor(color1, immediate ? 0 : 0.15f);
                }
                onUpdate.Invoke(false);
            }
        }
    }
}