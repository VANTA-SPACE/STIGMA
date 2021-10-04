using System;
using System.Collections.Generic;
using Core;
using Core.Level;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Manager {
    public class PlayManager : MonoBehaviour {
        public static PlayManager Instance => _instance;
        private static PlayManager _instance;

        public NoteGenerator generator;
        public double rawBeat;
        public double beatOffset;
        public double currentBeat => rawBeat - beatOffset;
        public bool isPlayingLevel;
        public double currentBpm;
        public Text exampleText;
        public Image pausePanel;
        public bool Paused { get; private set; }
        
        public LevelData levelData = null;

        public JudgmentLine JudgmentLine => JudgmentLine.Instance;

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            pausePanel.gameObject.SetActive(false);
        }

        public void LoadLevel(LevelData levelData) {
            this.levelData = levelData;
            currentBpm = this.levelData.BPM;
            const int beatDelay = 2;
            Instance.beatOffset = -levelData.Offset * levelData.BPM / 6000d + beatDelay;
            SoundManager.Instance.PlayEvent(levelData.EventName, () => currentBeat >= levelData.Offset * levelData.BPM / 6000);
            generator.GenerateNotes(levelData.NoteDatas);
            //Other tasks
        }

        public void StartPlay() {
            isPlayingLevel = true;
            rawBeat = 0;
            LoadLevel(levelData);
            Debug.Log("Started Playing");
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
            if (!Paused) {
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

        public void TogglePause() {
            Debug.Log("Toggle Pause");
            if (Paused) UnpauseGame();
            else PauseGame();
        }
        
        public void PauseGame() {
            Paused = true;
            SoundManager.Instance.Pause();
            pausePanel.gameObject.SetActive(true);
            pausePanel.DOColor(new Color(0, 0, 0, 0.8f), 0.1f);
        }
        
        public void UnpauseGame() {
            Paused = false;
            SoundManager.Instance.Unpause();
            pausePanel.gameObject.SetActive(false);
            pausePanel.DOColor(Color.clear, 0.1f);
        }
    }
}