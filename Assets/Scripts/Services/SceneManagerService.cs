using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LaserChess
{
    public class SceneManagerService : MonoBehaviour
    {
        private int unloadedScenesCount;

        public void LoadNewSceneState(string initialSceneName, string newSceneName)
        {
            UnloadAdditiveScenes(initialSceneName, () => StartLoadScene(newSceneName));
        }

        private void UnloadAdditiveScenes(string initialSceneName, Action nextAction = null)
        {
            unloadedScenesCount = 0;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name != initialSceneName)
                    StartCoroutine(UnloadScene(SceneManager.GetSceneAt(i).name));
            }

            StartCoroutine(CheckUnloadedScenes(nextAction));
        }

        private IEnumerator UnloadScene(string sceneName)
        {
            // unload new scene
            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            unloadedScenesCount++;
        }

        private IEnumerator CheckUnloadedScenes(Action nextAction)
        {
            while (unloadedScenesCount < SceneManager.sceneCount - 1)
                yield return null;
            
            nextAction?.Invoke();
        }

        private void StartLoadScene(string newSceneName)
        {
            StartCoroutine(LoadScene(newSceneName));
        }

        private IEnumerator LoadScene(string newSceneName)
        {
            // load new scene
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}