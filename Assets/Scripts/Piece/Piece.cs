using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LaserChess
{
    public partial class Piece : MonoBehaviour
    {
        public PieceType pieceType;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;

        public PieceInfo pieceInfo;

        public Vector2Int cellCoordinates { get; set; }

        [Header("Piece State")]
        private PieceStateType pieceStateType;
        public PieceStateType PieceStateType { get { return pieceStateType; } }

        private Action onCompleteAnimationAction;

        private int health;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = _navMeshAgent.GetComponent<Animator>();

            _animator.applyRootMotion = true;
            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateRotation = false;

            destination = Vector3.forward;

            pieceStateType = PieceStateType.Idle;

            health = pieceInfo.hitPoints;

            CreateAnimations();
        }

        public void Reset()
        {
            pieceStateType = PieceStateType.Idle;
        }

        private void Update()
        {
            UpdateAnimations();
        }

        public bool TakeDamage(int damage)
        {
            health -=  damage;

            if (health <= 0)
            {
                pieceStateType = PieceStateType.Die;
                PlayDieAnimation();

                return true;
            }
            else
            {
                PlayHitAnimation();
            }

            return false;
        }

        Vector3 GetWorldDirection()
        {
            return (destination - transform.localPosition).normalized;
        }

        Vector3 GetTurnDirection()
        {
            return transform.InverseTransformDirection(GetWorldDirection());
        }

        Quaternion GetDirectionToTarget()
        {
            Vector3 worldDirection = GetWorldDirection();
            float angle = Mathf.Atan2(worldDirection.x, worldDirection.z);
            return Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.up);
        }

        public void MoveAgent(Vector3 nextDestination, Vector2Int nextCellCoordinates)
        {
            destination = nextDestination;
            cellCoordinates = nextCellCoordinates;

            _navMeshAgent.SetDestination(destination);

            pieceStateType = PieceStateType.Move;

            StartMove();
        }

        public void Attack()
        {
            pieceStateType = PieceStateType.Attack;

            StartAttack();
        }

        public void OnFinishDie()
        {
            Destroy(this.gameObject);
        }
    }

    public enum PieceType
    {
        Grunt,
        Jumpship,
        Tank,
        Drone,
        Dreadnought,
        CommandUnit
    }

    public enum PieceStateType
    {
        Idle,
        Move,
        Attack,
        Die,
        Complete
    }
}