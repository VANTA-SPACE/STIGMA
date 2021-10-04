using Core.Level;
using DG.Tweening;
using Manager;
using UnityEngine;
using Utils;

namespace Core {
    public class NoteNormal : NoteBase {
        //public ParticleSystem noteParticle;
        public override void Init(NoteData data) {
            Data = data;
            TargetBeat = Data.StartBeat;
            GetPositionInternal = (x, y) => new Vector2(x, y) + new Vector2(0, Distance * Constants.NOTE_SPEED_MODIFIER).Rotate(transform.eulerAngles.z);
            NotePos = data.NotePos;
            JudgmentLine.AssignedNotes[NotePos].Enqueue(this);
            Inited = true;
        }

        public override Judgment CheckJudgment() {
            double timeOffset = -Distance / PlayManager.Instance.currentBpm * 1000 / 60;
            return StigmaUtils.GetJudgement(timeOffset);
        }
        
        public override bool CheckMiss() {
            double timeOffset = -Distance / PlayManager.Instance.currentBpm * 1000 / 60;
            return StigmaUtils.CheckMiss(timeOffset);
        }

        public override Vector2 GetPosition() {
            var pos = JudgmentLine.Positions[NotePos];
            Distance = (float) (TargetBeat - CurrBeat);
            return GetPositionInternal(pos.x, pos.y);
        }

        public override void MissNote() {
            var sprite = GetComponent<SpriteRenderer>();
            //noteParticle.Stop();
            sprite.DOColor(new Color(1, 1, 1, 0), 0.5f);
        }
        
        public override void DestroyNote(Judgment judgment) {
            var sprite = GetComponent<SpriteRenderer>();
            sprite.enabled = false;
            //noteParticle.Stop();
        }
    }
}