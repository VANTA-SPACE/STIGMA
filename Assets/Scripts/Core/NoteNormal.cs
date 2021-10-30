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

        public GameObject noteParticlePrefabPerfect;
        public GameObject noteParticlePrefabElse;
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
            
            transform.position = GetPosition();
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
            ShowNoteParticle(judgment);
            //noteParticle.Stop();
        }

        public void ShowJudgementText(Judgment judgment) {
            PlayManager.Instance.CheckedNotes++;
            switch (judgment) {
                case Judgment.PerfectEarly:
                    PlayManager.Instance.JudgmentCount[Judgment.PerfectEarly]++;
                    PlayManager.Instance.GaugeValue += Constants.PERFECTEL_TOTALGAUGE / PlayManager.Instance.TotalNotes;
                    goto case Judgment.Perfect;

                case Judgment.PerfectLate:
                    PlayManager.Instance.JudgmentCount[Judgment.PerfectEarly]++;
                    PlayManager.Instance.GaugeValue += Constants.PERFECTEL_TOTALGAUGE / PlayManager.Instance.TotalNotes;
                    goto case Judgment.Perfect;

                case Judgment.Perfect:
                    if (judgment == Judgment.Perfect) {
                        PlayManager.Instance.GaugeValue += Constants.PERFECT_TOTALGAUGE / PlayManager.Instance.TotalNotes;
                    }

                    PlayManager.Instance.JudgmentCount[Judgment.Perfect]++;
                    PlayManager.Instance.Accurary += 100;
                    PlayManager.Instance.Combo++;
                    PlayManager.Instance.Score += 1000000f;
                    break;
                
                case Judgment.GreatEarly:
                    PlayManager.Instance.JudgmentCount[Judgment.GreatEarly]++;
                    goto case Judgment.Great;

                case Judgment.GreatLate:
                    PlayManager.Instance.JudgmentCount[Judgment.GreatLate]++;
                    goto case Judgment.Great;

                case Judgment.Great:
                    PlayManager.Instance.JudgmentCount[Judgment.Great]++;
                    PlayManager.Instance.Accurary += 90;
                    PlayManager.Instance.Combo++;
                    PlayManager.Instance.Score += 900000f;
                    PlayManager.Instance.GaugeValue += Constants.GOOD_FIXEDGAUGE;
                    break;

                case Judgment.GoodEarly:
                    PlayManager.Instance.JudgmentCount[Judgment.GoodEarly]++;
                    goto case Judgment.Good;

                case Judgment.GoodLate:
                    PlayManager.Instance.JudgmentCount[Judgment.GoodLate]++;
                    goto case Judgment.Good;

                case Judgment.Good:
                    PlayManager.Instance.JudgmentCount[Judgment.Good]++;
                    PlayManager.Instance.Accurary += 60;
                    PlayManager.Instance.Combo++;
                    PlayManager.Instance.Score += 600000f;
                    PlayManager.Instance.GaugeValue += Constants.GOOD_FIXEDGAUGE;
                    break;

                case Judgment.Bad:
                    PlayManager.Instance.JudgmentCount[Judgment.Bad]++;
                    PlayManager.Instance.Accurary += 20;
                    PlayManager.Instance.Combo = 0;
                    PlayManager.Instance.Score += 200000f;
                    PlayManager.Instance.GaugeValue += Constants.BAD_TOTALGAUGE / PlayManager.Instance.TotalNotes;
                    break;

                case Judgment.Miss:
                    PlayManager.Instance.JudgmentCount[Judgment.Miss]++;
                    PlayManager.Instance.Combo = 0;
                    PlayManager.Instance.GaugeValue += Constants.MISS_TOTALGAUGE / PlayManager.Instance.TotalNotes;
                    break;

                default:
                    Debug.Log(judgment.ToString());
                    throw new ArgumentOutOfRangeException(nameof(judgment));
            }

            var obj = Instantiate(judgmentPrefab);
            obj.transform.position = new Vector3(transform.position.x, -2f);
            var tmp = obj.transform.GetChild(0).GetComponent<TMP_Text>();
            // if (judgment == Judgment.Perfect) {
            //     tmp.colorGradient = new VertexGradient(new Color(1, 1, 0.6f), new Color(0.9f, 0.6f, 1)
            //         , new Color(0.9f, 0.6f, 1), new Color(0.9f, 0.6f, 1));
            //     tmp.enableVertexGradient = true;
            // } else {
            //     tmp.color = Constants.JudgmentColors[judgment];
            //     tmp.enableVertexGradient = false;
            // }

            switch (judgment) {
                case Judgment.Perfect:
                case Judgment.PerfectEarly:
                case Judgment.PerfectLate:
                    tmp.colorGradient = new VertexGradient(new Color(1, 1, 0.6f), new Color(0.9f, 0.6f, 1)
                        , new Color(0.9f, 0.6f, 1), new Color(0.9f, 0.6f, 1));
                    tmp.enableVertexGradient = true;
                    break;
                case Judgment.Great:
                case Judgment.GreatEarly:
                case Judgment.GreatLate:
                    tmp.color = Constants.JudgmentColors[Judgment.Great];
                    tmp.enableVertexGradient = false;
                    break;
                case Judgment.Good:
                case Judgment.GoodEarly:
                case Judgment.GoodLate:
                    tmp.color = Constants.JudgmentColors[Judgment.Good];
                    tmp.enableVertexGradient = false;
                    break;
                default:
                    tmp.color = Constants.JudgmentColors[judgment];
                    tmp.enableVertexGradient = false;
                    break;
            }

            var judge = judgment.ToString().SplitCapital().Split(' ');

            // original - PERIOT
            // if (judge.Length == 1) tmp.text = judge[0].ToUpper();
            // else if (judge.Length == 2) {
            //     if (judge[0] == "Good") tmp.text = judge[1].ToUpper();
            //     if (judge[0] == "Perfect" && !Settings.ShowELOnPerfect) tmp.text = judge[0].ToUpper();
            //     else tmp.text = judge[0].ToUpper() + "\n" + judge[1].ToLower();
            // }

            // EDIT NO EL - TILTO
            tmp.text = judge[0].ToUpper();
        }

        public void ShowNoteParticle(Judgment judgment) {
            GameObject obj;
            if (judgment == Judgment.Perfect || judgment == Judgment.PerfectEarly || judgment == Judgment.PerfectLate) {
                obj = Instantiate(noteParticlePrefabPerfect);
            } else {
                obj = Instantiate(noteParticlePrefabElse);
            }

            obj.transform.position = transform.position;
            noteParticle = obj.GetComponent<ParticleSystem>();
            DOTween.Sequence().AppendCallback(() => Destroy(obj)).SetDelay(2);
        }
    }
}