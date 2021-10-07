using System;
using System.Collections.Generic;
using System.Linq;
using Core.Level;
using Manager;
using UnityEngine;
using Utils;

namespace Core {
    public class JudgmentLine : MonoBehaviour {
        public Dictionary<NotePos, Queue<NoteBase>> AssignedNotes;
        public static JudgmentLine Instance => _instance;
        private static JudgmentLine _instance;
        public Dictionary<NotePos, Vector2> Positions;

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            AssignedNotes = new Dictionary<NotePos, Queue<NoteBase>> {
                {NotePos.POS_0, new Queue<NoteBase>()},
                {NotePos.POS_1, new Queue<NoteBase>()},
                {NotePos.POS_2, new Queue<NoteBase>()},
                {NotePos.POS_3, new Queue<NoteBase>()},
            };
            Positions = new Dictionary<NotePos, Vector2>();
        }

        // Update is called once per frame
        void Update() {
            if (PlayManager.Instance.isPlayingLevel) {
                var position = transform.position;
                var x = position.x;
                var y = position.y;
                var angle = transform.eulerAngles.z;
                Positions[NotePos.POS_0] =
                    new Vector2(x, y) + new Vector2(Constants.NOTE_WIDTH * -1.5f, 0).Rotate(angle);
                Positions[NotePos.POS_1] =
                    new Vector2(x, y) + new Vector2(Constants.NOTE_WIDTH * -0.5f, 0).Rotate(angle);
                Positions[NotePos.POS_2] =
                    new Vector2(x, y) + new Vector2(Constants.NOTE_WIDTH * 0.5f, 0).Rotate(angle);
                Positions[NotePos.POS_3] =
                    new Vector2(x, y) + new Vector2(Constants.NOTE_WIDTH * 1.5f, 0).Rotate(angle);

                foreach (var (key, queue) in AssignedNotes) {
                    if (!queue.Any()) continue;
                    if (queue.Peek().CheckMiss()) queue.Dequeue().MissNote();
                }

                CheckKeyPress();
            }
        }

        public void CheckKeyPress() {
            foreach (var (pos, queue) in AssignedNotes) {
                KeyCode keyCode;
                switch (pos) {
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
                        continue;
                }
                if (!Input.GetKeyDown(keyCode)) continue;
                
                if (!queue.Any()) continue;
                var judgment = queue.Peek().CheckJudgment();
                if (judgment == Judgment.None) return;
                if (!queue.Peek().HasLength) {
                    queue.Dequeue().DestroyNote(judgment);
                    return;
                }

                StartCoroutine(((NoteLong) queue.Dequeue()).CheckJudgmentCo());
            }
        }
    }
}