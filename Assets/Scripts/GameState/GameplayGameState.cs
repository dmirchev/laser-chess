using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class GameplayGameState : GameState<GameplayGameStateUI>
    {
        public override GameStateType GetGameStateType()
        {
            return GameStateType.Gameplay;
        }

        // [Header("Gameplay")]
    }
}