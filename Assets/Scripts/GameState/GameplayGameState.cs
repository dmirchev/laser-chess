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

        [Header("Gameplay")]
        [SerializeField] public Level level;

        [Header("Gameplay State")]
        private GameplayState currentGameplayState;

        private SelectGameplayState selectGameplayState;
        private MoveGameplayState moveGameplayState;
        private AttackGameplayState attackGameplayState;

        public static bool isPlayer;

        void Awake()
        {
            currentGameplayState = null;

            selectGameplayState = new SelectGameplayState(this);
            moveGameplayState = new MoveGameplayState(this);
            attackGameplayState = new AttackGameplayState(this);

            isPlayer = true;
        }

        public override void Enter()
        {
            base.Enter();

            level.Init();

            SetGameplayState(GameplayStateType.Select);

            gameStateUI.SetHeader(isPlayer);
        }

        public void CheckIsPlayerFinished()
        {
            if (level.CheckIsPlayerFinished(out gameManager.hasPlayerWon))
                LoadNextState();

            gameStateUI.SetHeader(isPlayer);
        }

        public void SetGameplayState(GameplayStateType gameplayStateType)
        {
            if (gameplayStateType == GameplayStateType.None) return;

            if (currentGameplayState != null)
                currentGameplayState.Exit();
            
            currentGameplayState = GetGameplayState(gameplayStateType);
            currentGameplayState.Enter();
        }

        public void GetInput(RaycastHit hit, bool isAPiece)
        {
            currentGameplayState.GetInput(hit, isAPiece);
        }

        public void NoInput()
        {
            currentGameplayState.NoInput();
        }

        public void ForceNextGameplayState()
        {
            currentGameplayState.NextGameplayState();
        }

        public void SetNextButtonState(bool isActive, string text)
        {
            gameStateUI.SetNextStateButton(isActive, text);
        }

        // Get
        private GameplayState GetGameplayState(GameplayStateType gameplayStateType)
        {
            switch (gameplayStateType)
            {
                case GameplayStateType.Select: return selectGameplayState;
                case GameplayStateType.Move: return moveGameplayState;
                case GameplayStateType.Attack: return attackGameplayState;
            }

            return null;
        }
    }
}