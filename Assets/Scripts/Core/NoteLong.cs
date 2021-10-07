using System;
using System.Collections;
using Core.Level;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using Utils;

namespace Core {
    public class NoteLong : NoteBase {
        public override bool HasLength => true;

        public TrailRenderer trail;
        public GameObject judgmentPrefab;
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
            trail.time = (float) data.NoteLength / 1000;
            Pressing = false;
            Inited = true;
        }

        public override Judgment CheckJudgment() {
            double timeOffset = -Distance * BeatToSecond;
            var judgment = StigmaUtils.GetJudgement(timeOffset);
            return judgment;
        }

        public IEnumerator CheckJudgmentCo() {
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
            ShowNoteParticle();
            var color = GetComponent<SpriteRenderer>().color;
            while (Input.GetKey(keyCode)) {
                double to = (float) ((EndMilisec - CurrMilisec) * MilisecToBeat);
                if (StigmaUtils.GetJudgement(to) == Judgment.None) {
                    break;
                }

                var width = Mathf.Max((float) ((EndMilisec - CurrMilisec) / (EndMilisec - TargetMilisec)), 0) * MilisecToBeatF;
                var eased = DOVirtual.EasedValue(0, 1, width, Ease.OutExpo);
                trail.startColor = trail.endColor = new Color(color.r, color.g, color.b, eased / 3 + 0.2f);
                yield return null;
            }

            HideNoteParticle();
            
            double timeOffset = (float) ((EndMilisec - CurrMilisec) * MilisecToBeat);
            var positive = timeOffset <= 0;
            timeOffset = (float) (Math.Abs(startTimeOffset) / 4 + Math.Abs(timeOffset)) / 4;
            var judgment = StigmaUtils.GetJudgement(timeOffset * (positive ? 1 : -1));
            if (judgment == Judgment.None || judgment == Judgment.Miss) judgment = Judgment.Bad;
            DestroyNote(judgment);
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
            DOTween.To(() => trail.startColor, c => trail.startColor = trail.endColor = c, color, 0.5f);
            ShowJudgementText(Judgment.Miss);
        }

        public override void DestroyNote(Judgment judgment) {
            var sprite = GetComponent<SpriteRenderer>();
            var color = Color.Lerp(sprite.color, Color.clear, 0.5f);
            sprite.DOColor(color, 0.5f);
            DOTween.To(() => trail.startColor, c => trail.startColor = trail.endColor = c, color, 0.5f);
            ShowJudgementText(judgment);
            //noteParticle.Stop();
        }

        protected override void Update() {
            if (!trail.enabled) trail.enabled = true;
            base.Update();
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
            obj.transform.position = new Vector3(transform.position.x, JudgmentLine.transform.position.y);
            noteParticle = obj.GetComponent<ParticleSystem>();
        }

        public void HideNoteParticle() {
            noteParticle.Stop();
            StartCoroutine(StigmaUtils.SetDelay(() => Destroy(noteParticle.gameObject), 4));
        }
    }
}