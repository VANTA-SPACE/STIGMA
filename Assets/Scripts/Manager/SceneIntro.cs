using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Manager {
    public class SceneIntro : MonoBehaviour {
        
        public static SceneIntro Instance { get; private set; }
        
        public static string SceneToLoad = Constants.PLAY_SCENE;
        public Image exitPanel;
        public RectTransform exitMenu;
        public bool showMenu;

        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            exitPanel.gameObject.SetActive(false);
            exitMenu.gameObject.SetActive(false);
            exitMenu.localScale = Vector3.zero;
        }

        private void Start()
        {
            if (SoundManager.Instance)
            {
                SoundManager.Instance.PlayLevelEvent("Scene_Intro");
            }
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (showMenu) HideExitMenu(); else ShowExitMenu();
            }
            if (GameManager.Instance.ValidAnyKeyDown(false)) {
                Debug.Log("STIGMA - SOUND CHANGING");
                SoundManager.Instance.EditParameter("MainState", 1.0f);
                Trans transition = Trans.FromUp | Trans.FromDown | Trans.ToLeft | Trans.ToRight;
                GameManager.Instance.LoadScene(SceneToLoad, transition);
            }
        }
        
        public void ShowExitMenu() {
            showMenu = true;
            DOTween.Kill("UnExitGame", true);
            exitMenu.localScale = new Vector3(0, 0, 0);
            exitPanel.gameObject.SetActive(true);
            exitMenu.gameObject.SetActive(true);
            exitPanel.DOColor(new Color(0, 0, 0, 0.8f), 0.2f).SetId("ExitGame");
            exitMenu.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutExpo).SetId("ExitGame");
        }

        public void HideExitMenu() {
            showMenu = false;
            DOTween.Kill("ExitGame", true);
            exitPanel.DOColor(Color.clear, 0.2f).OnComplete(() => exitPanel.gameObject.SetActive(false))
                .SetId("UnpauseGame");
            exitMenu.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutExpo).SetId("UnExitGame").OnComplete(() => exitMenu.gameObject.SetActive(false));
        }
    }
}