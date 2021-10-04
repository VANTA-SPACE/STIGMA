using System;
using System.Collections.Generic;
using System.Linq;
using Core.Level;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Components {
    public class JudgmentLine : MonoBehaviour {
        public readonly Queue<NoteNormal> AssignedNotes = new Queue<NoteNormal>();
        public static JudgmentLine Instance;
        private static JudgmentLine _instance;
        public Vector2[] positions;

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            positions = new Vector2[4];
        }

        // Update is called once per frame
        void Update() {
            var position = transform.position;
            var x = position.x;
            var y = position.y;
            var angle = transform.eulerAngles.z;
            positions[0] = new Vector2(x, y) + new Vector2(Constants.NOTE_WIDTH * -1.5f, 0).Rotate(angle);
            positions[1] = new Vector2(x, y) + new Vector2(Constants.NOTE_WIDTH * -0.5f, 0).Rotate(angle);
            positions[2] = new Vector2(x, y) + new Vector2(Constants.NOTE_WIDTH * 0.5f, 0).Rotate(angle);
            positions[3] = new Vector2(x, y) + new Vector2(Constants.NOTE_WIDTH * 1.5f, 0).Rotate(angle);
            
            if (AssignedNotes.Any()) {
                if (AssignedNotes.Peek().CheckMiss()) {
                    AssignedNotes.Dequeue().MissNote();
                }
            }
        }

        public void OnKeyPress() {
            if (AssignedNotes.Any()) {
                var judgment = AssignedNotes.Peek().CheckJudgment();
                if (judgment == Judgment.None) return;
                AssignedNotes.Peek().DestroyNote(judgment);
            }
        }
    }
}