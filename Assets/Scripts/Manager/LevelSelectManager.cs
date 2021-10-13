using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Level;
using Serialization;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
using Coroutine = Utils.Coroutine;

namespace Manager {
    public class LevelSelectManager : Manager<LevelSelectManager> {
        [Serializable]
        public struct PartialLevelData {
            public int id;
            public string name;
            public Sprite cover;
            public PartialLevelData(int id, string name, Sprite cover) {
                this.id = id;
                this.name = name;
                this.cover = cover;
            }
        }

        public Image coverImg;
        public TMP_Text levelNameText;
        public StartButton startButton;
        
        public string levelname;
        public RectTransform levelList;
        public GameObject loadButtonPrefab;
        public List<PartialLevelData> levels;

        public override void Init() {
            var levelDatas = (List<object>) Json.Deserialize(Resources.Load<TextAsset>("Levels/levels").text);
            foreach (Dictionary<string, object> level in levelDatas) {
                var covername = level["cover"].As<string>();
                var data = new PartialLevelData(level["id"].As<int>(), level["name"].As<string>(), Resources.Load<Sprite>(covername));
                levels.Add(data);
            }
            levels.Sort(data => data.id);

            foreach (var level in levels) {
                var levelBtn = Instantiate(loadButtonPrefab, levelList);
                var text = (TMP_Text) levelBtn.GetComponent<NamedComponent>().components["LevelName"];
                text.text = level.name;
                var cover = (Image) levelBtn.GetComponent<NamedComponent>().components["Cover"];
                cover.sprite = level.cover;
                var btn = levelBtn.GetComponent<RadioButton>();
                btn.id = "Levels";
                btn.value = level.id;
                btn.onUpdate.AddListener((value) => {
                    if (!value) {
                        var found = levels.Any(data => data.id == RadioButtonManager.Instance.Values["Levels"]);
                        startButton.interactable = found;
                        return;
                    }
                    coverImg.sprite = level.cover;
                    levelname = level.name;
                    levelNameText.text = level.name;
                    startButton.interactable = true;
                });
            }

            startButton.interactable = false;
            
            SoundManager.Instance.PlayEvent("Level_Selection");
        }

        public void StartLevel() {
            DontDestroyOnLoad(gameObject);
            SoundManager.Instance.StopEvent(false);
            GameManager.Instance.LoadScene(Constants.PLAY_SCENE, Trans.FadeStart | Trans.ToUp | Trans.ToDown);
            StartCoroutine(StartCo());
        }

        private IEnumerator StartCo() {
            yield return new WaitForSeconds(GameManager.Instance.transitionLength);
            yield return new WaitWhile(() => PlayManager.Instance == null);
            PlayManager.Instance.LoadLevel(levelname);
            Destroy(gameObject);
        }
    }
}