using System;
using System.Collections.Generic;
using Core.Level;
using UnityEngine;

namespace DebugObj {
    public class LevelEditor : MonoBehaviour {
        public LevelData LevelData = new LevelData(150, 0, new List<NoteData>(), "");
    }
}