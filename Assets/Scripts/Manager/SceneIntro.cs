using UnityEngine;

namespace Manager {
    public class SceneIntro : MonoBehaviour {
        public static string SceneToLoad = Constants.PLAY_SCENE;

        private void Update() {
            if (GameManager.Instance.ValidAnyKeyDown()) {
                var transition = Trans.FromUp | Trans.FromDown | Trans.ToLeft | Trans.ToRight;
                GameManager.Instance.LoadScene(SceneToLoad, transition);
            }
        }
    }
}