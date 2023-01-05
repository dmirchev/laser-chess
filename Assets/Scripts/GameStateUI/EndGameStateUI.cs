using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LaserChess
{
    public class EndGameStateUI : GameStateUI<EndGameState>
    {
        [Header("End")]
        public Button finishButton;
        public TMP_Text endGameText;
        public string playerWonString;
        public string aiWonString;

        protected override void OnAwake()
        {
            finishButton.onClick.RemoveAllListeners();
            finishButton.onClick.AddListener(OnClickFinishButton);
        }

        public void SetEndGameText(bool hasPlayerWon)
        {
            endGameText.text = hasPlayerWon ? playerWonString : aiWonString;
        }

        public void OnClickFinishButton()
        {
            gameState.LoadNextState();
        }
    }
}