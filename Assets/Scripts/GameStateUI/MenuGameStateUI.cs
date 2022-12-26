using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LaserChess
{
    public class MenuGameStateUI : GameStateUI<MenuGameState>
    {
        [SerializeField] private Button startuButton;
        
        protected override void OnAwake()
        {
            startuButton.onClick.RemoveAllListeners();
            startuButton.onClick.AddListener(OnClickStartButton);
        }

        public void OnClickStartButton()
        {
            gameState.LoadNextState();
        }
    }
}