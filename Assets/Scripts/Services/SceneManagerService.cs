using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LaserChess
{
    public class SceneManagerService : MonoBehaviour
    {
        public void LoadNewSceneState(string newSceneName)
        {
            StartCoroutine(LoadScene(newSceneName));
        }

        private IEnumerator LoadScene(string newSceneName)
        {
            // load new scene
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Single);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}