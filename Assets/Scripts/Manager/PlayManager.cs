using System;
using System.Collections.Generic;
using Core;
using Core.Level;
using DG.Tweening;
using Serialization;
using TMPro;
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
        public double CurrentBeat => rawBeat - beatOffset;
        public bool isPlayingLevel;
        public double currentBpm;
        public TMP_Text exampleText;
        public TMP_Text comboText;
        public TMP_Text scoreText;
        public TMP_Text spaceToPlay;
        public Image pausePanel;
        public RectTransform pauseMenu;
        public bool Paused { get; private set; }

        public float accurary;
        public int totalnote = 0;
        public int combo = 0;
        public float score = 0;

        public LevelData levelData = null;
        public List<Judgment> judgmentList = new List<Judgment>();
        public int totalMisses;

        public JudgmentLine JudgmentLine => JudgmentLine.Instance;

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            pausePanel.gameObject.SetActive(false);
            
            comboText.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(false);
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

        public void LoadLevel(string levelname) {
            var level = new LevelData(
                (Dictionary<string, object>) Json.Deserialize(Resources.Load<TextAsset>("Levels/" + levelname).text));
            LoadLevel(level);
        }

        public void StartPlay() {
            spaceToPlay.DOColor(new Color(1, 1, 1, 0), 0.25f);

            EndPlay();
            isPlayingLevel = true;
            rawBeat = 0;
            LoadLevel("exlevel");
            Debug.Log("Started Playing");

            accurary = 0;
            totalnote = 0;
            combo = 0;
            score = 0;

            scoreText.text = string.Empty;
            comboText.text = string.Empty;
            
            comboText.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);
            
        }

        public void EndPlay() {
            isPlayingLevel = false;
            rawBeat = 0;
            SoundManager.Instance.StopEvent();
            Debug.Log("Stopped Playing");
            for (int i = 0; i < generator.transform.childCount; i++) {
                Destroy(generator.transform.GetChild(i).gameObject);
            }
            comboText.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(false);
        }

        public void Retry() {
            EndPlay();
            spaceToPlay.color = Color.white;
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
            if (!Paused) {
                if (isPlayingLevel) {
                    rawBeat += currentBpm / 60d * Time.deltaTime;
                    
                    if (totalnote == 0) {
                        scoreText.text = "0";
                        comboText.text = $"Combo: <color=#ffff7f>{combo}</color>";
                    } else {
                        scoreText.text = score.ToString("#,###,##0");
                        if ((int) (accurary / totalnote) == 100) {
                            comboText.text = $"Combo: <color=#ffff7f>{combo}</color>";
                        } else if (totalMisses == 0) {
                            comboText.text = $"Combo: <color=#7fbfff>{combo}</color>";
                        } else {
                            comboText.text = $"Combo: {combo}";
                        }
                    }
                } else {
                    if (GameManager.Instance.ValidAnyKeyDown()) {
                        StartPlay();
                    }
                }
            }

            var curr = isPlayingLevel ? $"{CurrentBeat:0.0000}" : "Not playing";
            if (totalnote == 0) {
                exampleText.text = $"CurrentBeat: {curr}\nPaused: {Paused}\nAccurary: 100.00%";
            } else {
                exampleText.text = $"CurrentBeat: {curr}\nPaused: {Paused}\nAccurary: " +
                                   (accurary / totalnote).ToString("0.00") + "%";
            }
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
            pausePanel.DOColor(Color.clear, 0.2f).OnComplete(() => pausePanel.gameObject.SetActive(false))
                .SetId("UnpauseGame");
            pauseMenu.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutExpo).SetId("UnpauseGame");
        }
    }
}