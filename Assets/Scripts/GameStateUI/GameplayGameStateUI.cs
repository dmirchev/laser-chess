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
        [SerializeField] private Button nextStateButton;
        [SerializeField] private TMP_Text nextStateButtonText;
        [SerializeField] private TMP_Text headerText;

        [SerializeField] private string playerTurnText;
        [SerializeField] private string aiTurnText;

        [Header("Score")]
        [SerializeField] private TMP_Text scoreText;

        [Header("Player Turns Componentes")]
        [SerializeField] private TMP_Text leftPlayerText;
        [SerializeField] private TMP_Text rightPlayerText;
        [SerializeField] private GameObject leftPlayerTextGameObject;
        [SerializeField] private GameObject rightPlayerTextGameObject;
        [SerializeField] private PlayerTurnIndicatorUI leftPlayerTurnIndicatorUI;
        [SerializeField] private PlayerTurnIndicatorUI rightPlayerTurnIndicatorUI;

        void Awake()
        {
            nextStateButton.onClick.RemoveAllListeners();
            nextStateButton.onClick.AddListener(gameState.ForceNextGameplayState);

            leftPlayerText.text = playerTurnText;
            rightPlayerText.text = aiTurnText;

            leftPlayerTextGameObject.SetActive(false);
            rightPlayerTextGameObject.SetActive(false);
        }

        public void SetNextStateButton(bool isActive, string text)
        {
            nextStateButton.interactable = isActive;
            nextStateButtonText.text = text;
        }

        public void SetHeader(bool isPlayer, int leftPlayerPiecesLeft, int rightPlayerPiecesLeft)
        {
            headerText.text = isPlayer ? playerTurnText : aiTurnText;

            leftPlayerTextGameObject.SetActive(isPlayer);
            rightPlayerTextGameObject.SetActive(!isPlayer);

            leftPlayerTurnIndicatorUI.StartAnimation(isPlayer);
            rightPlayerTurnIndicatorUI.StartAnimation(!isPlayer);

            SetScore(leftPlayerPiecesLeft, rightPlayerPiecesLeft);
        }

        public void SetScore(int leftPlayer, int rightPlayer)
        {
            scoreText.text = $"{leftPlayer}/{rightPlayer}";
        }
    }
}