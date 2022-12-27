using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class LevelSelectionGameState : GameState<LevelSelectionGameStateUI>
    {
        public override GameStateType GetGameStateType()
        {
            return GameStateType.LevelSelection;
        }

        // [Header("Level Selection")]

        public void SetLevelIntex(int levelIndex)
        {
            gameManager.LevelIndex = levelIndex;

            LoadNextState();
        }
    }
}