using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MellowMadness.Core {

    public class SceneController : Singleton<SceneController> {

        [Header("Key Scenes")]

        [SerializeField] private int mainMenuIndex;
        [SerializeField] private int gameIndex;

        private int currentAdditiveScene = 1;
        
        #region Public Methods

        public void LoadGameScene() {
            StartCoroutine(LoadAdditiveSceneAsync(gameIndex));
        }

        public void LoadMainMenu() {
            StartCoroutine(LoadAdditiveSceneAsync(mainMenuIndex));
        }

        public void QuitGame() {
            Application.Quit();
        }

        #endregion

        #region Coroutines

        private IEnumerator LoadAdditiveSceneAsync(int sceneIndex) {
            if (currentAdditiveScene != 0) {
                var asyncUnload = SceneManager.UnloadSceneAsync(currentAdditiveScene);

                while (asyncUnload.isDone == false) {
                    yield return null;
                }
            }

            var asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

            while (asyncLoad.isDone == false) {
                yield return null;
            }

            currentAdditiveScene = sceneIndex;
        }

        #endregion

    }

}