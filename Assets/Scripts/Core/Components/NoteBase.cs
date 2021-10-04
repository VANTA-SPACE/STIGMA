using System;
using Core.Level;
using Manager;
using UnityEngine;

namespace Core.Components {
    public abstract class NoteBase : MonoBehaviour {
        public NoteData Data;
        public static double CurrBeat => GameManager.Instance.CurrentBeat;
        
        [NonSerialized] public double TargetBeat;
        [NonSerialized] public float Distance;
        [NonSerialized] public bool Inited = false;

        public JudgmentLine JudgmentLine => JudgmentLine.Instance;
        public abstract void Init(NoteData data);
        protected Func<float, float, Vector2> GetPositionInternal;
        public abstract Vector2 GetPosition();
        
        protected virtual void Update() {
            if (!Inited) return;
            transform.position = GetPosition();
        }

        public abstract void MissNote();
        public abstract void DestroyNote(Judgment judgment);
        public abstract Judgment CheckJudgment();
        public abstract bool CheckMiss();
    }
}