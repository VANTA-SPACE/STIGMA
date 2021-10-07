using System;
using Core.Level;
using Manager;
using UnityEngine;

namespace Core {
    public abstract class NoteBase : MonoBehaviour {
        public NoteData Data;
        public static double CurrMilisec => PlayManager.Instance.CurrentMilisec;
        
        [NonSerialized] public double TargetMilisec;
        [NonSerialized] public float Distance;
        [NonSerialized] public bool Inited = false;
        [NonSerialized] public NotePos NotePos;

        public double MilisecToBeat => Data.LevelData.Bpm / 60 / 1000;
        public float MilisecToBeatF => (float) Data.LevelData.Bpm / 60 / 1000;
        
        public double BeatToSecond => 60 / Data.LevelData.Bpm;
        public float BeatToSecondF => 60 / (float) Data.LevelData.Bpm;

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
        public abstract bool HasLength { get; }
    }
}