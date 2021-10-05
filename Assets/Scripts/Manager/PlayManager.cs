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
        public RectTransform pauseMenu;
        public bool Paused { get; private set; }
        
        public LevelData levelData = null; 
        public List<(Judgment judgment, int missCount)> judgmentList = new List<(Judgment judgment, int missCount)>();
        public int totalMisses;

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
            SoundManager.Instance.PlayMainEvent(levelData.EventName, levelData.Offset);
            generator.GenerateNotes(levelData.NoteDatas);
            //Other tasks
        }

        public void StartPlay() {
            EndPlay();
            isPlayingLevel = true;
            rawBeat = 0;
            LoadLevel(levelData);
            Debug.Log("Started Playing");
        }
        
        public void EndPlay() {
            isPlayingLevel = false;
            rawBeat = 0;
            SoundManager.Instance.StopEvent();
            Debug.Log("Stopped Playing");
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
            if (!Paused) {
                if (isPlayingLevel) {
                    rawBeat += currentBpm / 60d * Time.deltaTime;
                } else {
                    if (Input.GetKeyDown(KeyCode.Space)) {
                        StartPlay();
                    }
                }
            }

            var curr = isPlayingLevel ? $"{currentBeat}" : "Not playing";
            exampleText.text = $"CurrentBeat: {curr}\nPaused: {Paused}";
        }

        public void TogglePause() {
            Debug.Log("Toggle Pause");
            if (Paused) UnpauseGame();
            else PauseGame();
        }
        
        public void PauseGame() {
            DOTween.Kill("UnpauseGame", true);
            Paused = true;
            SoundManager.Instance.Pause();
            pauseMenu.localScale = new Vector3(0, 0, 0);
            pausePanel.gameObject.SetActive(true);
            pausePanel.DOColor(new Color(0, 0, 0, 0.8f), 0.2f).SetId("PauseGame");
            pauseMenu.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutExpo).SetId("PauseGame");
        }
        
        public void UnpauseGame() {
            DOTween.Kill("PauseGame", true);
            Paused = false;
            SoundManager.Instance.Unpause(true);
            pausePanel.DOColor(Color.clear, 0.2f).OnComplete(() => pausePanel.gameObject.SetActive(false)).SetId("UnpauseGame");
            pauseMenu.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutExpo).SetId("UnpauseGame");
        }
    }
}