using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private string initialSceneName;
        [SerializeField] private GameStateType initialGameStateType;

        [SerializeField] private SceneManagerService sceneManagerService;

        private int levelIndex;
        public int LevelIndex { get; set; }

        void Awake()
        {
            LoadNextGameState(initialGameStateType);
        }

        public void LoadNextGameState(GameStateType gameStateType)
        {
            sceneManagerService.LoadNewSceneState(initialSceneName, gameStateType.ToString());
        }
    }
}