using System.Collections.Generic;
using Core.Components;
using Core.Level;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Manager {
    public class PlayManager : MonoBehaviour {
        public static PlayManager Instance => _instance;
        private static PlayManager _instance;

        public NoteGenerator generator;
        public LevelData LevelData;
        public double rawBeat;
        public double beatOffset;
        public double currentBeat => rawBeat - beatOffset;
        public bool isPlayingLevel;
        public double currentBpm;
        public Text exampleText;

        public JudgmentLine JudgmentLine => JudgmentLine.Instance;

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        public void LoadLevel(LevelData levelData) {
            LevelData = levelData;
            currentBpm = LevelData.BPM;
            const int beatDelay = 2;
            Instance.beatOffset = -levelData.Offset * levelData.BPM / 6000d + beatDelay;
            SoundManager.Instance.PlayEvent(levelData.EventName, 60 / (float) levelData.BPM * beatDelay);
            generator.GenerateNotes(levelData.NoteDatas);
            //Other tasks
        }

        public void StartPlay() {
            isPlayingLevel = true;
            rawBeat = 0;
            LoadLevel(new LevelData(
                150, -25, new List<NoteData>() {
                    new NoteData(NoteType.Normal, 0, NotePos.POS_0),
                    new NoteData(NoteType.Normal, 1, NotePos.POS_1),
                    new NoteData(NoteType.Normal, 2, NotePos.POS_2),
                    new NoteData(NoteType.Normal, 3, NotePos.POS_3),
                }, Constants.TitleEvent));
            Debug.Log("Started Playing");
        }

        // Update is called once per frame
        void Update() {
            if (isPlayingLevel) {
                rawBeat += currentBpm / 60d * Time.deltaTime;
                exampleText.text = $"{currentBeat}";
            } else {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    StartPlay();
                }
            }
        }
    }
}