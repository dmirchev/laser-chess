using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LaserChess
{
    public class Piece : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera = null;
        [SerializeField]
        private LayerMask _layerMask;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;

        public bool turn;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = _navMeshAgent.GetComponent<Animator>();

            _animator.applyRootMotion = true;
            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateRotation = false;

            hitPoint = Vector3.forward;
        }

        private void OnAnimatorMove()
        {
            Vector3 rootPosition = _animator.rootPosition;
            rootPosition.y = _navMeshAgent.nextPosition.y;
            transform.position = rootPosition;
            transform.rotation = setOtherDir ? GetNewDir() : _animator.rootRotation;
            _navMeshAgent.nextPosition = rootPosition;
        }

        private void Update()
        {
            HandleInput();

            Turn();

            bool hasReachedEnd = _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;

            if (setOtherDir)
                setOtherDir = !hasReachedEnd;
            _animator.SetBool("stop", hasReachedEnd);
        }

        bool hasSecondTurn;

        Vector3 start;

        void Turn()
        {
            if (!turn) return;
            turn = false;

            setOtherDir = false;

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
                _animator.SetTrigger("isWalking");
                hasSecondTurn = false;
            }

            start = transform.localPosition;
            
            PlayTurnAnimation(nextTurnDirection);
        }

        Vector3 GetWorldDirection()
        {
            return (hitPoint - transform.localPosition).normalized;
        }

        Vector3 GetTurnDirection()
        {
            return transform.InverseTransformDirection(GetWorldDirection());
        }

        void PlayTurnAnimation(Vector3 direction)
        {
            _animator.SetFloat("dirX", direction.x);
            _animator.SetFloat("dirY", direction.z);

            _animator.SetTrigger("isTurning");
        }

        bool setOtherDir;

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

                setOtherDir = true;
            }
        }

        Quaternion GetNewDir()
        {
            Vector3 worldDirection = GetWorldDirection();
            float angle = Mathf.Atan2(worldDirection.x, worldDirection.z);
            return Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.up);
        }

        public Vector3 hitPoint;

        private void HandleInput()
        {
            if (Application.isFocused && Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, _layerMask))
                {
                    turn = true;
                    hitPoint = hit.point;
                    _navMeshAgent.SetDestination(hit.point);
                    Debug.DrawRay(hit.point, Vector3.up, Color.blue, 10.0f);
                }
            }
        }
    }
}