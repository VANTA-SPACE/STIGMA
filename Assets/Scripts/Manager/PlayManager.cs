using System;
using System.Collections.Generic;
using Core;
using Core.Level;
using DG.Tweening;
using Serialization;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Manager {
    public class PlayManager : MonoBehaviour {
        public static PlayManager Instance => instance;
        private static PlayManager instance;
        public static bool IsPlaying = false;

        public NoteGenerator generator;
        public TMP_Text exampleText;
        public TMP_Text comboText;
        public TMP_Text scoreText;
        public TMP_Text spaceToPlay;

        public Image pausePanel;
        public RectTransform pauseMenu;
        [NonSerialized] public bool Paused;

        [NonSerialized] public float Accurary;
        [NonSerialized] public int Combo;
        [NonSerialized] public float Score;
        [NonSerialized] public int CheckedNotes;

        public LevelData LevelData;
        public Dictionary<Judgment, int> JudgmentCount;

        private double currentRawMilisec;
        public double milisecOffset;
        public double CurrentMilisec => (currentRawMilisec - milisecOffset);
        public double BaseBpm => LevelData?.Bpm ?? 0;
        public double CurrentBeat => CurrentMilisec * BaseBpm / 60000;
        public bool isPlayingLevel;
        public double currentBpm;
        
        public int TotalMiss => JudgmentCount[Judgment.Miss];
        public int Totalnotes => LevelData.NoteDatas.Count;
                
        public double MilisecToBeat => BaseBpm / 60 / 1000;
        public float MilisecToBeatF => (float) BaseBpm / 60 / 1000;
        
        public double BeatToSecond => 60 / BaseBpm;
        public float BeatToSecondF => 60 / (float) BaseBpm;

        public JudgmentLine JudgmentLine => JudgmentLine.Instance;

        private void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this;
            pausePanel.gameObject.SetActive(false);

            comboText.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(false);
        }

        public void LoadLevel(LevelData levelData) {
            LevelData = levelData;
            currentBpm = BaseBpm;
            const int beatDelay = 2;
            milisecOffset = -levelData.Offset + beatDelay * 60 / BaseBpm * 1000;
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
            ProgressBar.instance.StartProgress();
            EndPlay();
            isPlayingLevel = true;
            currentRawMilisec = 0;
            LoadLevel("exlevel");
            Debug.Log("Started Playing");

            Accurary = 0;
            Combo = 0;
            Score = 0;
            CheckedNotes = 0;
            
            JudgmentCount = new Dictionary<Judgment, int>() {
                {Judgment.Perfect, 0},
                {Judgment.PerfectEarly, 0},
                {Judgment.PerfectLate, 0},
                {Judgment.Good, 0},
                {Judgment.GoodEarly, 0},
                {Judgment.GoodLate, 0},
                {Judgment.Bad, 0},
                {Judgment.Miss, 0},
            };

            scoreText.text = string.Empty;
            comboText.text = string.Empty;
            IsPlaying = true;
            comboText.gameObject.SetActive(IsPlaying);
            scoreText.gameObject.SetActive(IsPlaying);
        }

        public void EndPlay() {
            isPlayingLevel = false;
            currentRawMilisec = 0;
            SoundManager.Instance.StopEvent();
            Debug.Log("Stopped Playing");
            for (int i = 0; i < generator.transform.childCount; i++) {
                var obj = generator.transform.GetChild(i).gameObject;
                var longn = obj.GetComponent<NoteLong>()?.noteParticle;
                if (longn != null) {
                    Destroy(longn.gameObject);
                }
                Destroy(obj);
            }
            ProgressBar.instance.EndProgress();
            IsPlaying = false;
            comboText.gameObject.SetActive(IsPlaying);
            scoreText.gameObject.SetActive(IsPlaying);
        }

        public void Retry() {
            EndPlay();
            spaceToPlay.color = Color.white;
        }

        public void FinishGame() {
            EndPlay();
            ResultScreen.ShowResultScene();
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
            if (!Paused) {
                if (isPlayingLevel) {
                    currentRawMilisec += Time.deltaTime * 1000d;

                    if (CheckedNotes == 0) {
                        scoreText.text = "0";
                        comboText.text = $"Combo: <color=#ffff7f>{Combo}</color>";
                    } else {
                        scoreText.text = Score.ToString("#,###,##0");
                        if ((int) (Accurary / CheckedNotes) == 100) {
                            comboText.text = $"Combo: <color=#ffff7f>{Combo}</color>";
                        } else if (TotalMiss == 0) {
                            comboText.text = $"Combo: <color=#7fbfff>{Combo}</color>";
                        } else {
                            comboText.text = $"Combo: {Combo}";
                        }
                    }
                } else {
                    if (GameManager.Instance.ValidAnyKeyDown()) {
                        StartPlay();
                    }
                }
            }

            if (CheckedNotes == 0) {
                Accurary = 0;
            } else {
                Accurary = (JudgmentCount[Judgment.Perfect] + (JudgmentCount[Judgment.Good] * 0.7f) + (JudgmentCount[Judgment.Bad] * 0.3f)) * 100;
            }


            var curr = isPlayingLevel ? $"{CurrentBeat:0.0000}" : "Not playing";
            if (CheckedNotes == 0) {
                exampleText.text = $"CurrentBeat: {curr}\nPaused: {Paused}\nAccurary: 100.00%";
            } else {
                exampleText.text = $"CurrentBeat: {curr}\nPaused: {Paused}\nAccurary: " +
                                   (Accurary / CheckedNotes).ToString("0.00") + "%";
            }
            
            if (!isPlayingLevel || Paused || CurrentMilisec <= LevelData.EndMilisec + 4 * BeatToSecond * 1000) return;
            Debug.Log($"{CurrentMilisec} > {LevelData.EndMilisec + 4 * BeatToSecond * 1000}");
            FinishGame();
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