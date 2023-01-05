using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    [CreateAssetMenu(fileName = "Piece", menuName = "PieceScriptableObject", order = 51)]
    public class PieceInfo : ScriptableObject
    {
        public string label;

        public PieceGridBehaviour movePieceGridBehaviour;
        public PieceGridBehaviour attackPieceGridBehaviour;

        public AttackType attackType;
        public int attackPower;
        public int hitPoints;
        public bool playerControlled;
    }

    [System.Serializable]
    public class PieceGridBehaviour
    {
        public bool canLeap;
        public bool isInfinite;
        public List<Vector2Int> directionsOrPositions;
    }

    public enum AttackType
    {
        None,
        Once,
        All
    }
}