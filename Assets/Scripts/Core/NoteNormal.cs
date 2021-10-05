using Core.Level;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using Utils;

namespace Core {
    public class NoteNormal : NoteBase {
        //public ParticleSystem noteParticle;
        public GameObject judgmentPrefab;
        public override void Init(NoteData data) {
            Data = data;
            TargetBeat = Data.StartBeat;
            GetPositionInternal = (x, y) => new Vector2(x, y) + new Vector2(0, Distance * Constants.NOTE_SPEED_MODIFIER).Rotate(transform.eulerAngles.z);
            NotePos = data.NotePos;
            JudgmentLine.AssignedNotes[NotePos].Enqueue(this);
            Inited = true;
        }

        public override Judgment CheckJudgment() {
            double timeOffset = -Distance * 60 / PlayManager.Instance.currentBpm;
            return StigmaUtils.GetJudgement(timeOffset);
        }
        
        public override bool CheckMiss() {
            double timeOffset = -Distance * 60 / PlayManager.Instance.currentBpm;
            return StigmaUtils.CheckMiss(timeOffset);
        }

        public override Vector2 GetPosition() {
            var pos = JudgmentLine.Positions[NotePos];
            Distance = (float) (TargetBeat - CurrBeat);
            return GetPositionInternal(pos.x, pos.y);
        }

        public override void MissNote() {
            var sprite = GetComponent<SpriteRenderer>();
            sprite.DOColor(new Color(1, 1, 1, 0), 0.5f);
            ShowJudgementText(Judgment.Miss);
        }
        
        public override void DestroyNote(Judgment judgment) {
            Destroy(gameObject);
            ShowJudgementText(judgment);
            //noteParticle.Stop();
        }

        public void ShowJudgementText(Judgment judgment) {
            var obj = Instantiate(judgmentPrefab);
            obj.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f);
            var tmp = obj.transform.GetChild(0).GetComponent<TMP_Text>();
            if (judgment == Judgment.Perfect) {
                tmp.colorGradient = new VertexGradient(new Color(1, 1, 0.6f), new Color(0.9f, 0.6f, 1)
                    , new Color(0.9f, 0.6f, 1), new Color(0.9f, 0.6f, 1));
                tmp.enableVertexGradient = true;
            } else {
                tmp.color = Constants.JudgmentColors[judgment];
                tmp.enableVertexGradient = false;
            }
            var judge = judgment.ToString().SplitCapital().Split(' ');
            
            if (judge.Length == 1) tmp.text = judge[0].ToUpper();
            else if (judge.Length == 2) {
                if (judge[0] == "Good") tmp.text = judge[1].ToUpper();
                else tmp.text = judge[0].ToUpper() + "\n" + judge[1].ToLower();
            }
        }
    }
}