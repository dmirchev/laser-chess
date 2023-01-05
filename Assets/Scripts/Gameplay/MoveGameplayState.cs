using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class MoveGameplayState : GameplayState
    {
        public override GameplayStateType GetGameplayStateType()
        {
            return GameplayStateType.Move;
        }

        public override GameplayStateType GetNextGameplayStateType()
        {
            return GameplayStateType.Attack;
        }

        public override GameplayStateType GetPreviousGameplayStateType()
        {
            return GameplayStateType.Select;
        }

        public MoveGameplayState(GameplayGameState gameplayGameState) : base (gameplayGameState) { }

        public override void Enter()
        {
            stopInput = false;

            state.SetNextButtonState(true, "Attack");
        }

        public override void Exit()
        {
            
        }

        public override void GetInput(RaycastHit hit, bool isAPiece)
        {
            if (stopInput) return;

            if (state.level.HasSelectedSamePiece(hit, isAPiece))
            {
                PreviousGameplayState();
                return;
            }

            if (state.level.IsCellAvailableToMove(hit, isAPiece))
            {
                state.level.SelectedPiece.SetOnCompleteAnimationAction(NextGameplayState);
                stopInput = true;
            }

            state.SetNextButtonState(false, "Wait");
        }

        public override void NoInput()
        {
            state.level.ClearSelectedPiece();
            PreviousGameplayState();
        }
    }
}