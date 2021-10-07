using System;
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
            TargetMilisec = Data.StartMilisec;
            GetPositionInternal = (x, y) =>
                new Vector2(x, y) +
                new Vector2(0, Distance * Constants.NOTE_SPEED_MODIFIER).Rotate(transform.eulerAngles.z);
            NotePos = data.NotePos;
            JudgmentLine.AssignedNotes[NotePos].Enqueue(this);
            Inited = true;
        }

        public override Judgment CheckJudgment() {
            double timeOffset = -Distance * BeatToSecond;
            var judgment = StigmaUtils.GetJudgement(timeOffset);
            if (judgment == Judgment.None) return Judgment.None;

            return judgment;
        }

        public override bool CheckMiss() {
            double timeOffset = -Distance * BeatToSecond;
            return StigmaUtils.CheckMiss(timeOffset);
        }

        public override Vector2 GetPosition() {
            var pos = JudgmentLine.Positions[NotePos];
            Distance = (float) ((TargetMilisec - CurrMilisec) * MilisecToBeat);
            return GetPositionInternal(pos.x, pos.y);
        }

        public override void MissNote() {
            var sprite = GetComponent<SpriteRenderer>();
            sprite.DOColor(new Color(1, 1, 1, 0), 0.5f);
            ShowJudgementText(Judgment.Miss);
        }

        public override void DestroyNote(Judgment judgment) {
            if (gameObject == null) return;
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
                if (judge[0] == "Perfect" && !Settings.ShowELOnPerfect) tmp.text = judge[0].ToUpper();
                else tmp.text = judge[0].ToUpper() + "\n" + judge[1].ToLower();
            }
           
            PlayManager.Instance.CheckedNotes++;
            switch (judgment) {
                case Judgment.PerfectEarly:
                    PlayManager.Instance.JudgmentCount[Judgment.PerfectEarly]++;
                    goto case Judgment.Perfect;
                    
                case Judgment.PerfectLate:
                    PlayManager.Instance.JudgmentCount[Judgment.PerfectEarly]++;
                    goto case Judgment.Perfect;
                    
                case Judgment.Perfect:
                    PlayManager.Instance.JudgmentCount[Judgment.Perfect]++;
                    PlayManager.Instance.Accurary += 100;
                    PlayManager.Instance.Combo++;
                    PlayManager.Instance.Score += 1000000f / PlayManager.Instance.LevelData.NoteDatas.Count;
                    break;
                
                case Judgment.GoodEarly:
                    PlayManager.Instance.JudgmentCount[Judgment.GoodEarly]++;
                    goto case Judgment.Good;
                    
                case Judgment.GoodLate:
                    PlayManager.Instance.JudgmentCount[Judgment.GoodLate]++;
                    goto case Judgment.Good;
                    
                case Judgment.Good:
                    PlayManager.Instance.JudgmentCount[Judgment.Good]++;
                    PlayManager.Instance.Accurary += 70;
                    PlayManager.Instance.Combo++;
                    PlayManager.Instance.Score += 700000f / PlayManager.Instance.LevelData.NoteDatas.Count;
                    break;
                
                case Judgment.Bad:
                    PlayManager.Instance.JudgmentCount[Judgment.Bad]++;
                    PlayManager.Instance.Accurary += 30;
                    PlayManager.Instance.Combo = 0;
                    PlayManager.Instance.Score += 300000f / PlayManager.Instance.LevelData.NoteDatas.Count;
                    break;
                
                case Judgment.Miss:
                    PlayManager.Instance.JudgmentCount[Judgment.Miss]++;
                    PlayManager.Instance.Combo = 0;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(judgment));
            }
        }
        
        public void ShowNoteParticle() {
            var obj = Instantiate(noteParticle.gameObject);
            obj.transform.position = transform.position;
            DOTween.Sequence().AppendCallback(() => Destroy(obj)).SetDelay(2);
        }
    }
}