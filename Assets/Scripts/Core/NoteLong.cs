using System;
using System.Collections;
using Core.Level;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core {
    public class NoteLong : NoteBase {
        public override bool HasLength => true;

        public Image trail;
        public GameObject judgmentPrefab;
        public GameObject noteParticlePrefabPerfect;
        public GameObject noteParticlePrefabElse;
        public ParticleSystem noteParticle;

        [NonSerialized] public double EndMilisec;
        [NonSerialized] public bool Pressing;

        public override void Init(NoteData data) {
            Data = data;
            TargetMilisec = Data.StartMilisec;
            EndMilisec = Data.EndMilisec;
            GetPositionInternal = (x, y) =>
                new Vector2(x, y) +
                new Vector2(0, Distance * Constants.NOTE_SPEED_MODIFIER).Rotate(transform.eulerAngles.z);
            NotePos = data.NotePos;
            JudgmentLine.AssignedNotes[NotePos].Enqueue(this);
            trail.enabled = false;
            trail.GetComponent<RectTransform>().sizeDelta =
                new Vector2(1, (float) data.NoteLength * MilisecToBeatF * Constants.NOTE_SPEED_MODIFIER);
            Pressing = false;
            Inited = true;
            
            transform.position = GetPosition();
        }

        public override Judgment CheckJudgment() {
            double timeOffset = -Distance * BeatToSecond;
            var judgment = StigmaUtils.GetJudgement(timeOffset);
            return judgment;
        }

        public IEnumerator CheckJudgmentCo(Judgment judgment) {
            var startTimeOffset = -Distance * BeatToSecond;
            Pressing = true;
            KeyCode keyCode;
            switch (Data.NotePos) {
                case NotePos.POS_0:
                    keyCode = Settings.Pos0Keycode;
                    break;
                case NotePos.POS_1:
                    keyCode = Settings.Pos1Keycode;
                    break;
                case NotePos.POS_2:
                    keyCode = Settings.Pos2Keycode;
                    break;
                case NotePos.POS_3:
                    keyCode = Settings.Pos3Keycode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(keyCode));
            }

            ShowNoteParticle(judgment);
            var color = GetComponent<SpriteRenderer>().color;
            while (Input.GetKey(keyCode)) {
                double to = (float) ((EndMilisec - CurrMilisec) * MilisecToBeat);
                if (StigmaUtils.GetJudgement(to) == Judgment.None) {
                    break;
                }

                var width = Mathf.Max((float) ((EndMilisec - CurrMilisec) / (EndMilisec - TargetMilisec)), 0) *
                            MilisecToBeatF;
                var eased = DOVirtual.EasedValue(0, 1, width, Ease.OutExpo);
                trail.color = new Color(color.r, color.g, color.b, eased / 3 + 0.2f);
                yield return null;
            }

            HideNoteParticle();

            double timeOffset = (float) ((EndMilisec - CurrMilisec) * MilisecToBeat);
            var positive = timeOffset <= 0;
            timeOffset = (float) (Math.Abs(startTimeOffset) / 4 + Math.Abs(timeOffset)) / 4;
            var _judgment = StigmaUtils.GetJudgement(timeOffset * (positive ? 1 : -1));
            if (_judgment == Judgment.None || _judgment == Judgment.Miss) _judgment = Judgment.Bad;
            DestroyNote(_judgment);
        }

        public override bool CheckMiss() {
            if (Pressing) return false;
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
            var color = Color.Lerp(sprite.color, Color.clear, 0.5f);
            sprite.DOColor(color, 0.5f);
            trail.DOColor(color, 0.5f);
            ShowJudgementText(Judgment.Miss);
        }

        public override void DestroyNote(Judgment judgment) {
            var sprite = GetComponent<SpriteRenderer>();
            var color = Color.Lerp(sprite.color, Color.clear, 0.5f);
            sprite.DOColor(color, 0.5f);
            trail.DOColor(color, 0.5f);
            ShowJudgementText(judgment);
            //noteParticle.Stop();
        }

        private void Start() {
            trail.enabled = true;
        }

        public void ShowJudgementText(Judgment judgment) {
            var obj = Instantiate(judgmentPrefab);
            obj.transform.position = new Vector3(transform.position.x, -2f);
            var tmp = obj.transform.GetChild(0).GetComponent<TMP_Text>();
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

            string[] judge = judgment.ToString().SplitCapital().Split(' ');

            tmp.text = judge.Length switch {
                1 => judge[0].ToUpper(),
                2 when judge[0] == "Good" => judge[1].ToUpper(),
                2 => judge[0].ToUpper() + "\n" + judge[1].ToLower(),
                _ => tmp.text
            };

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
                    PlayManager.Instance.Score += 1000000f / PlayManager.Instance.TotalNotes;
                    break;
                
                case Judgment.GreatEarly:
                    PlayManager.Instance.JudgmentCount[Judgment.GreatEarly]++;
                    goto case Judgment.Great;

                case Judgment.GreatLate:
                    PlayManager.Instance.JudgmentCount[Judgment.GreatLate]++;
                    goto case Judgment.Great;

                case Judgment.Great:
                    PlayManager.Instance.JudgmentCount[Judgment.Great]++;
                    PlayManager.Instance.Accurary += 75;
                    PlayManager.Instance.Combo++;
                    PlayManager.Instance.Score += 750000f / PlayManager.Instance.TotalNotes;
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
                    PlayManager.Instance.Accurary += 50;
                    PlayManager.Instance.Combo++;
                    PlayManager.Instance.Score += 500000f / PlayManager.Instance.TotalNotes;
                    PlayManager.Instance.GaugeValue += Constants.GOOD_FIXEDGAUGE;
                    break;

                case Judgment.Bad:
                    PlayManager.Instance.JudgmentCount[Judgment.Bad]++;
                    PlayManager.Instance.Accurary += 25;
                    PlayManager.Instance.Combo = 0;
                    PlayManager.Instance.Score += 250000f / PlayManager.Instance.TotalNotes;
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
        }


        public void ShowNoteParticle(Judgment judgment) {
            GameObject obj;
            if (judgment == Judgment.Perfect || judgment == Judgment.PerfectEarly || judgment == Judgment.PerfectLate) {
                obj = Instantiate(noteParticlePrefabPerfect);
            } else {
                obj = Instantiate(noteParticlePrefabElse);
            }

            obj.transform.position = new Vector3(transform.position.x, JudgmentLine.transform.position.y);
            noteParticle = obj.GetComponent<ParticleSystem>();
        }

        public void HideNoteParticle() {
            noteParticle.Stop();
            StartCoroutine(StigmaUtils.SetDelay(() => Destroy(noteParticle.gameObject), 4));
        }
    }
}