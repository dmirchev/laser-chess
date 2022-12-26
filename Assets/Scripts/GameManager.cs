using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameStateType initialGameStateType;

        [SerializeField] private SceneManagerService sceneManagerService;
        [SerializeField] private GameObject eventSystemGameObject;

        void Awake()
        {
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(eventSystemGameObject);

            LoadNextGameState(initialGameStateType);
        }

        public void LoadNextGameState(GameStateType gameStateType)
        {
            sceneManagerService.LoadNewSceneState(gameStateType.ToString());
        }
    }
}