using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public partial class Piece
    {
        [Header("Animation")]
        private bool isWalking;
        private bool hasSecondTurn;
        private Vector3 destination;

        int dieAnimationHash;
        int hitAnimationHash;

        private void CreateAnimations()
        {
            dieAnimationHash = Animator.StringToHash("Die");
            hitAnimationHash = Animator.StringToHash("Hit");
        }

        private void OnAnimatorMove()
        {
            Vector3 rootPosition = _animator.rootPosition;
            rootPosition.y = _navMeshAgent.nextPosition.y;
            transform.position = rootPosition;
            transform.rotation = isWalking ? GetDirectionToTarget() : _animator.rootRotation;
            _navMeshAgent.nextPosition = rootPosition;
        }

        public void SetOnCompleteAnimationAction(Action onComplete)
        {
            onCompleteAnimationAction = onComplete;
        }

        void UpdateAnimations()
        {
            if (pieceStateType == PieceStateType.Move)
            {
                if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                {
                    if (!isWalking) return;

                    isWalking = false;
                    _animator.SetTrigger("stopWalking");

                    pieceStateType = PieceStateType.Idle;

                    onCompleteAnimationAction?.Invoke();
                }
            }
        }

        void StartMove()
        {
            isWalking = false;

            Vector3 nextTurnDirection = GetTurnDirection();
            if (nextTurnDirection.z < 0)
            {
                nextTurnDirection.x = Mathf.Sign(nextTurnDirection.x);
                nextTurnDirection.z = 0;
                _animator.SetTrigger("hasSecondTurn");
                hasSecondTurn = true;
            }
            else
            {
                // _animator.SetTrigger("isWalking");
                hasSecondTurn = false;
                // isWalking = true;
            }
            
            PlayTurnAnimation(nextTurnDirection);
        }

        void PlayTurnAnimation(Vector3 direction)
        {
            _animator.SetFloat("dirX", direction.x);
            _animator.SetFloat("dirY", direction.z);

            _animator.SetTrigger("isTurning");
        }

        // End Of Turn Animation
        public void CheckAngle()
        {
            if (hasSecondTurn)
            {
                hasSecondTurn = false;
                PlayTurnAnimation(GetTurnDirection());
            }
            else
            {
                _animator.SetTrigger("isWalking");

                isWalking = true;
            }
        }

        void StartAttack()
        {
            _animator.SetTrigger("attack");
        }

        // End Of Attack Animation
        public void FinishAttack()
        {
            pieceStateType = PieceStateType.Complete;

            onCompleteAnimationAction?.Invoke();
        }

        void PlayDieAnimation()
        {
            _animator.Play(dieAnimationHash);
        }

        void PlayHitAnimation()
        {
            _animator.Play(hitAnimationHash);
        }
    }
}