using Core.Level;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using Utils;

namespace Core {
    public class NoteNormal : NoteBase {
        public override bool HasLength => false;

        public ParticleSystem noteParticle;
        public GameObject judgmentPrefab;

        public override void Init(NoteData data) {
            Data = data;
            TargetBeat = Data.StartBeat;
            GetPositionInternal = (x, y) =>
                new Vector2(x, y) +
                new Vector2(0, Distance * Constants.NOTE_SPEED_MODIFIER).Rotate(transform.eulerAngles.z);
            NotePos = data.NotePos;
            JudgmentLine.AssignedNotes[NotePos].Enqueue(this);
            Inited = true;
        }

        public override Judgment CheckJudgment() {
            double timeOffset = -Distance * 60 / PlayManager.Instance.currentBpm;
            var judgment = StigmaUtils.GetJudgement(timeOffset);
            if (judgment == Judgment.None) return Judgment.None;
            PlayManager.Instance.judgmentList.Add(judgment);

            return judgment;
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
            ShowNoteParticle();
            //noteParticle.Stop();
        }

        public void ShowJudgementText(Judgment judgment) {
            var obj = Instantiate(judgmentPrefab);
            obj.transform.position = new Vector3(transform.position.x, 1f);
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

            if (judgment == Judgment.Perfect || judgment == Judgment.PerfectEarly || judgment == Judgment.PerfectLate)
            {
                PlayManager.Instance.accurary += 100;
                PlayManager.Instance.combo += 1;
                PlayManager.Instance.score += 1000000f / PlayManager.Instance.levelData.NoteDatas.Count;
                if (judgment == Judgment.PerfectEarly)
                {
                    PlayManager.Instance.totalPerfect += 1;
                    PlayManager.Instance.totalPerfectE += 1;
                }
                else if (judgment == Judgment.PerfectLate)
                {
                    PlayManager.Instance.totalPerfect += 1;
                    PlayManager.Instance.totalPerfectL += 1;
                }
                else
                {
                    PlayManager.Instance.totalPerfect += 1;
                }
            }
            else if (judgment == Judgment.Good || judgment == Judgment.GoodEarly || judgment == Judgment.GoodLate)
            {
                PlayManager.Instance.accurary += 70;
                PlayManager.Instance.combo += 1;
                PlayManager.Instance.score += 700000f / PlayManager.Instance.levelData.NoteDatas.Count;
                if (judgment == Judgment.GoodEarly)
                {
                    PlayManager.Instance.totalGood += 1;
                    PlayManager.Instance.totalGoodE += 1;
                }
                else if (judgment == Judgment.GoodLate)
                {
                    PlayManager.Instance.totalGood += 1;
                    PlayManager.Instance.totalGoodL += 1;
                }
                else
                {
                    PlayManager.Instance.totalGood += 1;
                }
            }
            else if (judgment == Judgment.Bad)
            {
                PlayManager.Instance.accurary += 30;
                PlayManager.Instance.combo = 0;
                PlayManager.Instance.score += 300000f / PlayManager.Instance.levelData.NoteDatas.Count;
                PlayManager.Instance.totalGood += 1;
            }
            else
            {
                PlayManager.Instance.combo = 0;
            }
        }
        
        public void ShowNoteParticle() {
            var obj = Instantiate(noteParticle.gameObject);
            obj.transform.position = transform.position;
        }
    }
}