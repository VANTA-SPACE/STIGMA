using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class MainMenu : Manager<MainMenu> {
        public static string SceneToLoad = Constants.LEVEL_SELECT_SCENE;
        private void Start()
        {
            if (SoundManager.Instance)
            {
                SoundManager.Instance.PlayEvent("Level_Selection");
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SoundManager.Instance.EditParameter("MainState", 1.0f);
                const Trans transition = Trans.FadeStart | Trans.ToUp | Trans.ToDown;
                GameManager.Instance.LoadScene(SceneToLoad, transition);
            }

        }
    }
}
