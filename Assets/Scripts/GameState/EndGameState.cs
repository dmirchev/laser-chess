using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class EndGameState : GameState<EndGameStateUI>
    {
        public override GameStateType GetGameStateType()
        {
            return GameStateType.End;
        }

        // [Header("End")]

        public override void Enter()
        {
            base.Enter();

            gameStateUI.SetEndGameText(gameManager.hasPlayerWon);
        }
    }
}