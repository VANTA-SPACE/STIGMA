using Core.Level;
using DG.Tweening;
using Manager;
using UnityEngine;

namespace Core.Components {
    public class NoteNormal : NoteBase {
        public ParticleSystem noteParticle;
        public override void Init(NoteData data) {
            Data = data;
            TargetBeat = Data.StartBeat;
            GetPositionInternal = (x, y) => new Vector2(x, y + Distance);
            transform.eulerAngles = new Vector3(0, 0, 90);
            JudgmentLine.AssignedNotes.Enqueue(this);
            Inited = true;
        }

        public override Judgment CheckJudgment() {
            double timeOffset = -Distance / GameManager.Instance.CurrentBPM * 1000 / 60;
            return StigmaUtils.GetJudgement(timeOffset);
        }
        
        public override bool CheckMiss() {
            double timeOffset = -Distance / GameManager.Instance.CurrentBPM * 1000 / 60;
            return StigmaUtils.CheckMiss(timeOffset);
        }

        public override Vector2 GetPosition() {
            float x = 0;
            float y = 0;
            Distance = (float) (TargetBeat - CurrBeat);
            return GetPositionInternal(x, y);
        }

        public override void MissNote() {
            var sprite = GetComponent<SpriteRenderer>();
            noteParticle.Stop();
            sprite.DOColor(new Color(1, 1, 1, 0), 0.5f);
        }
        
        public override void DestroyNote(Judgment judgment) {
            var sprite = GetComponent<SpriteRenderer>();
            sprite.enabled = false;
            noteParticle.Stop();
        }
    }
}