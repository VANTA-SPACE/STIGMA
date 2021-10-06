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

        [NonSerialized] public double EndBeat;
        [NonSerialized] public bool Pressing = false;

        public override void Init(NoteData data) {
            Data = data;
            TargetBeat = Data.StartBeat;
            GetPositionInternal = (x, y) =>
                new Vector2(x, y) +
                new Vector2(0, Distance * Constants.NOTE_SPEED_MODIFIER).Rotate(transform.eulerAngles.z);
            NotePos = data.NotePos;
            JudgmentLine.AssignedNotes[NotePos].Enqueue(this);
            trail.enabled = false;
            EndBeat = data["EndBeat"].As(data.StartBeat);
            Pressing = false;
            Inited = true;
        }

        public override Judgment CheckJudgment() {
            double timeOffset = -Distance * 60 / PlayManager.Instance.currentBpm;
            var judgment = StigmaUtils.GetJudgement(timeOffset);
            return judgment;
        }

        public IEnumerator CheckJudgmentCo() {
            var startTimeOffset = -Distance * 60 / PlayManager.Instance.currentBpm;
            Pressing = true;
            var key = Settings.Keymap[Data.NotePos];
            ShowNoteParticle();
            var color = GetComponent<SpriteRenderer>().color;
            while (Input.GetKey(key)) {
                var width = Mathf.Max((float) ((EndBeat - CurrBeat) / (EndBeat - TargetBeat)));
                var eased = DOVirtual.EasedValue(0, 1, width, Ease.OutExpo);
                trail.startColor = trail.endColor = new Color(color.r, color.g, color.b, eased / 3 + 0.2f);
                yield return null;
            }
            HideNoteParticle();


            double timeOffset = (EndBeat - CurrBeat) * 60 / PlayManager.Instance.currentBpm;
            var positive = timeOffset <= 0;
            timeOffset = (float) (Math.Abs(startTimeOffset) + Math.Abs(timeOffset)) / 2;

            var judgment = StigmaUtils.GetJudgement(timeOffset * (positive ? 1 : -1));

            if (judgment == Judgment.None || judgment == Judgment.Miss) judgment = Judgment.Bad;
            DestroyNote(judgment);
        }

        public override bool CheckMiss() {
            if (Pressing) return false;
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
            var color = Color.Lerp(sprite.color, Color.clear, 0.5f);
            sprite.DOColor(color, 0.5f);
            DOTween.To(() => trail.startColor, c => trail.startColor = trail.endColor = c, color, 0.5f);
            ShowJudgementText(Judgment.Miss);
        }

        public override void DestroyNote(Judgment judgment) {
            var renderer = GetComponent<SpriteRenderer>();
            renderer.color = Color.Lerp(renderer.color, Color.clear, 0.8f);
            ShowJudgementText(judgment);
            //noteParticle.Stop();
        }

        protected override void Update() {
            base.Update();
            if (trail.enabled == false) {
                trail.enabled = true;
                trail.time = (float) ((EndBeat - TargetBeat) * 60 / PlayManager.Instance.currentBpm);
            }
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

            if (judgment == Judgment.Perfect || judgment == Judgment.PerfectEarly || judgment == Judgment.PerfectLate) {
                PlayManager.Instance.accurary += 100;
                PlayManager.Instance.totalnote += 1;
                PlayManager.Instance.combo += 1;
                PlayManager.Instance.score += 1000000f / PlayManager.Instance.levelData.NoteDatas.Count;
            } else if (judgment == Judgment.Good || judgment == Judgment.GoodEarly || judgment == Judgment.GoodLate) {
                PlayManager.Instance.accurary += 70;
                PlayManager.Instance.totalnote += 1;
                PlayManager.Instance.combo += 1;
                PlayManager.Instance.score += 700000f / PlayManager.Instance.levelData.NoteDatas.Count;
            } else if (judgment == Judgment.Bad) {
                PlayManager.Instance.accurary += 30;
                PlayManager.Instance.totalnote += 1;
                PlayManager.Instance.combo = 0;
                PlayManager.Instance.score += 300000f / PlayManager.Instance.levelData.NoteDatas.Count;
            } else {
                PlayManager.Instance.totalnote += 1;
                PlayManager.Instance.combo = 0;
            }
        }


        public void ShowNoteParticle() {
            var obj = Instantiate(noteParticle.gameObject);
            obj.transform.position = new Vector3(transform.position.x, 0);
            noteParticle = obj.GetComponent<ParticleSystem>();
        }

        public void HideNoteParticle() {
            noteParticle.Stop();
        }
    }
}