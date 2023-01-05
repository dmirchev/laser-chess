using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public abstract class GameplayState
    {
        public abstract GameplayStateType GetGameplayStateType();
        public abstract GameplayStateType GetNextGameplayStateType();
        public abstract GameplayStateType GetPreviousGameplayStateType();

        protected GameplayGameState state;
        protected bool stopInput;

        public GameplayState(GameplayGameState gameplayGameState)
        {
            state = gameplayGameState;
        }

        public virtual void Enter() => stopInput = false;
        public abstract void Exit();

        public void NextGameplayState() => state.SetGameplayState(GetNextGameplayStateType());
        public void PreviousGameplayState() => state.SetGameplayState(GetPreviousGameplayStateType());

        public abstract void GetInput(RaycastHit hit, bool isAPiece);
        public virtual void NoInput() { return; }
    }

    public enum GameplayStateType
    {
        None,
        Select,
        Move,
        Attack
    }
}