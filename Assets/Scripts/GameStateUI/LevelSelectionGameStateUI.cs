using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LaserChess
{
    public class LevelSelectionGameStateUI : GameStateUI<LevelSelectionGameState>
    {
        [Header("Level Selection")]
        public Button[] levels;

        protected override void OnAwake()
        {
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i].onClick.RemoveAllListeners();
                int levelIndex = i;
                levels[i].onClick.AddListener(() => PlayLevel(levelIndex));
            }
        }

        public void PlayLevel(int levelIndex)
        {
            gameState.SetLevelIntex(levelIndex);
        }
    }
}