using System.Collections.Generic;
using System.Linq;
using Core.Level;
using Manager;
using UnityEngine;
using Utils;

namespace Core {
    public class JudgmentLine : MonoBehaviour {
        public Dictionary<NotePos, Queue<NoteNormal>> AssignedNotes;
        public static JudgmentLine Instance => _instance;
        private static JudgmentLine _instance;
        public Dictionary<NotePos, Vector2> Positions;

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            AssignedNotes = new Dictionary<NotePos, Queue<NoteNormal>> {
                {NotePos.POS_0, new Queue<NoteNormal>()},
                {NotePos.POS_1, new Queue<NoteNormal>()},
                {NotePos.POS_2, new Queue<NoteNormal>()},
                {NotePos.POS_3, new Queue<NoteNormal>()},
            };
            Positions = new Dictionary<NotePos, Vector2>();
        }

        // Update is called once per frame
        void Update() {
            var position = transform.position;
            var x = position.x;
            var y = position.y;
            var angle = transform.eulerAngles.z;
            Positions[NotePos.POS_0] = new Vector2(x, y) + new Vector2(Constants.NOTE_WIDTH * -1.5f, 0).Rotate(angle);
            Positions[NotePos.POS_1] = new Vector2(x, y) + new Vector2(Constants.NOTE_WIDTH * -0.5f, 0).Rotate(angle);
            Positions[NotePos.POS_2] = new Vector2(x, y) + new Vector2(Constants.NOTE_WIDTH * 0.5f, 0).Rotate(angle);
            Positions[NotePos.POS_3] = new Vector2(x, y) + new Vector2(Constants.NOTE_WIDTH * 1.5f, 0).Rotate(angle);

            foreach (var (key, queue) in AssignedNotes) {
                if (queue.Any()) {
                    if (queue.Peek().CheckMiss()) {
                        queue.Dequeue().MissNote();
                        PlayManager.Instance.judgmentList.Add(Judgment.Miss);
                        PlayManager.Instance.totalMisses += 1;
                    }
                }
            }
            
            CheckKeyPress();
        }

        public void CheckKeyPress() {
            foreach (var (pos, queue) in AssignedNotes) {
                if (!Input.GetKeyDown(Settings.Keymap[pos])) {
                    continue;
                }

                Debug.Log($"Key {Settings.Keymap[pos]} pressed");
                if (!queue.Any()) continue;
                
                var judgment = queue.Peek().CheckJudgment();
                if (judgment == Judgment.None) return;
                queue.Dequeue().DestroyNote(judgment);
                PlayManager.Instance.judgmentList.Add(judgment);
            }
        }
    }
}