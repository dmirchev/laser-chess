using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class SelectGameplayState : GameplayState
    {
        public override GameplayStateType GetGameplayStateType()
        {
            return GameplayStateType.Select;
        }

        public override GameplayStateType GetNextGameplayStateType()
        {
            return GameplayStateType.Move;
        }

        public override GameplayStateType GetPreviousGameplayStateType()
        {
            return GameplayStateType.None;
        }

        public SelectGameplayState(GameplayGameState gameplayGameState) : base (gameplayGameState) { }

        public override void Enter()
        {
            base.Enter();
            state.level.ClearSelectedPiece();

            state.SetNextButtonState(false, "Select Piece");
        }

        public override void Exit()
        {
            
        }

        public override void GetInput(RaycastHit hit, bool isAPiece)
        {
            state.level.SelectOwnPiece(hit, isAPiece);

            if (state.level.SelectedPiece != null)
                NextGameplayState();
        }
    }
}