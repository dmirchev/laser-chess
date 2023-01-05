using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LaserChess
{
    public class GameplayGameStateUI : GameStateUI<GameplayGameState>
    {
        [Header("Gameplay")]
        public Button nextStateButton;
        public TMP_Text nextStateButtonText;

        void Awake()
        {
            nextStateButton.onClick.RemoveAllListeners();
            nextStateButton.onClick.AddListener(gameState.ForceNextGameplayState);
        }
    }
}