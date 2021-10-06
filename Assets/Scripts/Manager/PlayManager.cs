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
        public TMP_Text scoreText2;
        public TMP_Text spaceToPlay;

        public TMP_Text PerfectText;
        public TMP_Text GoodText;
        public TMP_Text BadText;
        public TMP_Text MissText;

        public TMP_Text FCText;

        public TMP_Text AccText;
        public TMP_Text RankText;

        public Image pausePanel;
        public RectTransform pauseMenu;
        public bool Paused { get; private set; }

        public float accurary;
        public int totalnote = 0;
        public int combo = 0;
        public float score = 0;

        public LevelData levelData = null;
        public List<Judgment> judgmentList = new List<Judgment>();
        public int totalPerfectE = 0;
        public int totalPerfect = 0;
        public int totalPerfectL = 0;
        public int totalGoodE = 0;
        public int totalGood = 0;
        public int totalGoodL = 0;
        public int totalBad = 0;
        public int totalMiss = 0;

        public GameObject ResultCanvas;

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
            totalPerfectE = 0;
            totalPerfect = 0;
            totalPerfectL = 0;
            totalGoodE = 0;
            totalGood = 0;
            totalGoodL = 0;
            totalBad = 0;
            totalMiss = 0;

            scoreText.text = string.Empty;
            comboText.text = string.Empty;
            
            comboText.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);

            judgmentList = null; 
            judgmentList = new List<Judgment>();

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
                        } else if (totalMiss == 0) {
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

            totalnote = totalPerfect + totalGood + totalBad + totalMiss;

            if (totalnote == 0)
            {
                accurary = 0;
            }
            else
            {
                accurary = (totalPerfect + (totalGood * 0.7f) + (totalBad * 0.3f)) * 100;
                AccText.text = (accurary / totalnote).ToString("###.00") + "%";
                if (accurary / totalnote >= 100)
                {
                    RankText.text = $"<color=#4D45FF>Ϛ</color>";
                }
                else if (accurary / totalnote >= 97)
                {
                    RankText.text = $"<color=#ff9f1f>Σ</color>";
                }
                else if (accurary / totalnote >= 95)
                {
                    RankText.text = $"<color=#ffdb3f>S</color>";
                }
                else if (accurary / totalnote >= 90)
                {
                    RankText.text = $"<color=#7fff00>A</color>";
                }
                else if (accurary / totalnote >= 80)
                {
                    RankText.text = $"<color=#2fffaf>B</color>";
                }
                else if (accurary / totalnote >= 70)
                {
                    RankText.text = $"<color=#2f9fff>C</color>";
                }
                else
                {
                    RankText.text = $"<color=#ff3f1f>D</color>";
                }

                if (accurary / totalnote >= 100)
                {
                    FCText.text = $"<color=#ffdb3f>AP</color>";
                }
                else if (totalMiss == 0)
                {
                    FCText.text = $"<color=#2f9fff>FC</color>";
                }
                else
                {
                    FCText.text = $"";
                }
            }


            var curr = isPlayingLevel ? $"{CurrentBeat:0.0000}" : "Not playing";
            if (totalnote == 0) {
                exampleText.text = $"CurrentBeat: {curr}\nPaused: {Paused}\nAccurary: 100.00%";
            } else {
                exampleText.text = $"CurrentBeat: {curr}\nPaused: {Paused}\nAccurary: " +
                                   (accurary / totalnote).ToString("0.00") + "%";
            }

            PerfectText.text = $"<size=36>(E{totalPerfectE} / L{totalPerfectL})</size> {totalPerfect}";
            GoodText.text = $"<size=36>(E{totalGoodE} / L{totalGoodL})</size> {totalGood}";
            BadText.text = $"{totalBad}";
            MissText.text = $"{totalMiss}";
            scoreText2.text = score.ToString("#,###,##0");

            

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