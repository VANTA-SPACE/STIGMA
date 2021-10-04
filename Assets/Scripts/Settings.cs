using System.Collections.Generic;
using Core.Level;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Settings {
    public static Dictionary<NotePos, KeyCode> Keymap = new Dictionary<NotePos, KeyCode> {
        {NotePos.POS_0, KeyCode.D},
        {NotePos.POS_1, KeyCode.F},
        {NotePos.POS_2, KeyCode.J},
        {NotePos.POS_3, KeyCode.K},
    };
}