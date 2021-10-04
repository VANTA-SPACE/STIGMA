using System;
using Core.Level;
using Manager;
using UnityEngine;

namespace Core {
    public abstract class NoteBase : MonoBehaviour {
        public NoteData Data;
        public static double CurrBeat => PlayManager.Instance.currentBeat;
        
        [NonSerialized] public double TargetBeat;
        [NonSerialized] public float Distance;
        [NonSerialized] public bool Inited = false;
        [NonSerialized] public NotePos NotePos;

        public JudgmentLine JudgmentLine => JudgmentLine.Instance;
        public abstract void Init(NoteData data);
        protected Func<float, float, Vector2> GetPositionInternal;
        public abstract Vector2 GetPosition();
        
        // ReSharper disable once LocalVariableHidesMember
        protected virtual void Update() {
            if (!Inited) return;
            var transform = this.transform;
            transform.position = GetPosition();
            transform.eulerAngles = JudgmentLine.transform.eulerAngles;
        }

        public abstract void MissNote();
        public abstract void DestroyNote(Judgment judgment);
        public abstract Judgment CheckJudgment();
        public abstract bool CheckMiss();
    }
}