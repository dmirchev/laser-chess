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
        public TMP_Text headerText;

        public string playerTurnText;
        public string aiTurnText;

        void Awake()
        {
            nextStateButton.onClick.RemoveAllListeners();
            nextStateButton.onClick.AddListener(gameState.ForceNextGameplayState);
        }

        public void SetNextStateButton(bool isActive, string text)
        {
            nextStateButton.interactable = isActive;
            nextStateButtonText.text = text;
        }

        public void SetHeader(bool isPlayer)
        {
            headerText.text = isPlayer ? playerTurnText : aiTurnText;
        }
    }
}