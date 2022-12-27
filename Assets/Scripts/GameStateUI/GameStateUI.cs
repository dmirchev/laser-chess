using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public abstract class GameStateUI<T> : MonoBehaviour where T : class
    {
        [Header("Game State UI")]
        [SerializeField] protected T gameState;

        void Awake() => OnAwake();

        protected virtual void OnAwake() { return; }
    }
}