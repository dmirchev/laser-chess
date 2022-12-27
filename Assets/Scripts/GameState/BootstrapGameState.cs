using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class BootstrapGameState : GameState<BootstrapGameStateUI>
    {
        public override GameStateType GetGameStateType()
        {
            return GameStateType.Bootstrap;
        }

        // [Header("Bootstrap")]

        public override void Enter()
        {
            base.Enter();

            LoadNextState();
        }
    }
}