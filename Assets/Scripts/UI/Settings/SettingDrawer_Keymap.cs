using System;
using System.Collections.Generic;
using System.Linq;
using Locale;
using Manager;
using Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace UI.Settings {
    // ReSharper disable once InconsistentNaming
    public class SettingDrawer_Keymap : SettingDrawer, IPointerClickHandler {
        public Button resetButton;
        public KeyCode defValue;

        public void OnPointerClick(PointerEventData eventData) {
            SettingScreen.Instance.CurrentKeymap = this;
        }

        public override void Init(SettingProperty property) {
            base.Init(property);
            defValue = (KeyCode) ((Dictionary<string, object>) global::Settings.SettingData[property.Category].GetOrDefault(property.Property))["default"];
            resetButton.onClick.AddListener(() => {
                Value = defValue;
                UpdateText();
                resetButton.gameObject.SetActive(false);
            });
            resetButton.gameObject.SetActive((KeyCode) Value != defValue);
        }

        public void CheckKey() {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) {
                if (!Input.GetKeyDown(key)) continue;
                if (key == KeyCode.Escape) {
                    Value = KeyCode.None;
                    return;
                }

                if (key == KeyCode.Mouse0 || key == KeyCode.Mouse1 || key == KeyCode.Mouse2 || key == KeyCode.Mouse3 ||
                    key == KeyCode.Mouse4 || key == KeyCode.Mouse5 || key == KeyCode.Mouse6) {
                    UpdateText();
                    SettingScreen.Instance.CurrentKeymap = null;
                    return;
                }
                Value = key;
                UpdateText();
                return;
            }

            text.text = $"> {GetText(Value)} <";
        }
        public override string GetText(object value) {
            resetButton.gameObject.SetActive((KeyCode) value != defValue);
            var key = (KeyCode) value;
            return key switch {
                KeyCode.None => "",
                KeyCode.Return => "Enter",
                KeyCode.Escape => "Esc",
                KeyCode.Space => "Space",
                KeyCode.UpArrow => "↑",
                KeyCode.DownArrow => "↓",
                KeyCode.RightArrow => "→",
                KeyCode.LeftArrow => "←",
                KeyCode.Alpha0 => "0",
                KeyCode.Alpha1 => "1",
                KeyCode.Alpha2 => "2",
                KeyCode.Alpha3 => "3",
                KeyCode.Alpha4 => "4",
                KeyCode.Alpha5 => "5",
                KeyCode.Alpha6 => "6",
                KeyCode.Alpha7 => "7",
                KeyCode.Alpha8 => "8",
                KeyCode.Alpha9 => "9",
                KeyCode.Exclaim => "!",
                KeyCode.DoubleQuote => "\"",
                KeyCode.Hash => "#",
                KeyCode.Dollar => "$",
                KeyCode.Percent => "%",
                KeyCode.Ampersand => "&",
                KeyCode.Quote => "'",
                KeyCode.LeftParen => "(",
                KeyCode.RightParen => ")",
                KeyCode.Asterisk => "*",
                KeyCode.Plus => "+",
                KeyCode.Comma => ",",
                KeyCode.Minus => "-",
                KeyCode.Period => ".",
                KeyCode.Slash => "/",
                KeyCode.Colon => ":",
                KeyCode.Semicolon => ";",
                KeyCode.Less => "<",
                KeyCode.Equals => "=",
                KeyCode.Greater => ">",
                KeyCode.Question => "?",
                KeyCode.At => "@",
                KeyCode.LeftBracket => "[",
                KeyCode.Backslash => "\\",
                KeyCode.RightBracket => "]",
                KeyCode.Caret => "^",
                KeyCode.Underscore => "_",
                KeyCode.BackQuote => "`",
                KeyCode.LeftCurlyBracket => "{",
                KeyCode.Pipe => "|",
                KeyCode.RightCurlyBracket => "}",
                KeyCode.Tilde => "~",
                KeyCode.Numlock => "Num Lock",
                KeyCode.CapsLock => "Caps Lock",
                KeyCode.ScrollLock => "Scroll Lock",
                KeyCode.RightShift => "RShift",
                KeyCode.LeftShift => "LShift",
                KeyCode.RightControl => "RControl",
                KeyCode.LeftControl => "LControl",
                KeyCode.RightAlt => "RAlt",
                KeyCode.LeftAlt => "LAlt",
                KeyCode.LeftCommand => "LCommnad",
                KeyCode.LeftWindows => "LWindows",
                KeyCode.RightCommand => "RCommand",
                KeyCode.RightWindows => "RWindows",
                _ => key.ToString()
            };
        }
    }
}