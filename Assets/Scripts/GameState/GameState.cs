using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public abstract class GameState<T> : MonoBehaviour where T : class
    {
        [Header("Game State")]
        protected GameManager gameManager;

        public abstract GameStateType GetGameStateType();

        [SerializeField] private GameStateType nextGameStateType;

        [SerializeField] protected T gameStateUI;

        void Start() => Enter();

        public virtual void Enter() => gameManager = FindObjectOfType<GameManager>();
        public virtual void Exit() { return; }

        public void LoadNextState() => gameManager.LoadNextGameState(nextGameStateType);
    }

    public enum GameStateType
    {
        Bootstrap,
        Menu,
        LevelSelection,
        Gameplay,
        End
    }
}