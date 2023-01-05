using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class InputHadler : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _layerMask;

        [SerializeField] private int pieceLayerIndex;

        public GameplayGameState gameplayGameState;

        void Awake()
        {
            gameplayGameState = FindObjectOfType<GameplayGameState>();
        }

        void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Application.isFocused && Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, _layerMask))
                    gameplayGameState.GetInput(hit, hit.transform.gameObject.layer == pieceLayerIndex);
                else
                    gameplayGameState.NoInput();
            }
        }
    }
}