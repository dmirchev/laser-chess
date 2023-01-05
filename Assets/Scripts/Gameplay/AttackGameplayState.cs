using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class AttackGameplayState : GameplayState
    {
        public override GameplayStateType GetGameplayStateType()
        {
            return GameplayStateType.Attack;
        }

        public override GameplayStateType GetNextGameplayStateType()
        {
            return GameplayStateType.Select;
        }

        public override GameplayStateType GetPreviousGameplayStateType()
        {
            return GameplayStateType.None;
        }

        public AttackGameplayState(GameplayGameState gameplayGameState) : base (gameplayGameState) { }

        public override void Enter()
        {
            base.Enter();

            state.level.ShowSquaresAttack();

            if (state.level.SelectedPiece.pieceInfo.attackType == AttackType.None)
            {
                stopInput = true;

                AttackNone();
                NextGameplayState();
                
                return;
            }
            else if (state.level.SelectedPiece.pieceInfo.attackType == AttackType.All)
            {
                stopInput = true;

                state.level.AreCellAvailableToAttackAll();
                SetAttack();
            }

            bool attackOnce = state.level.SelectedPiece.pieceInfo.attackType == AttackType.Once;
            state.SetNextButtonState(attackOnce, attackOnce ? "Skip" : "Wait");
        }

        public override void Exit()
        {
            if (!stopInput) AttackNone();

            state.CheckIsPlayerFinished();
        }

        public override void GetInput(RaycastHit hit, bool isAPiece)
        {
            if (stopInput) return;

            if (state.level.SelectedPiece.pieceInfo.attackType != AttackType.Once) return;

            if (state.level.IsCellAvailableToAttackOnce(hit, isAPiece))
            {
                SetAttack();
                stopInput = true;

                state.SetNextButtonState(false, "Wait");
            }
        }

        private void AttackNone()
        {
            state.level.AttackNone();
            SetAttack();
        }

        private void SetAttack()
        {
            state.level.SelectedPiece.SetOnCompleteAnimationAction(NextGameplayState);
        }
    }
}