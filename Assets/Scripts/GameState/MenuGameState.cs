using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class MenuGameState : GameState<MenuGameStateUI>
    {
        public override GameStateType GetGameStateType()
        {
            return GameStateType.Menu;
        }

        // [Header("Menu")]
    }
}