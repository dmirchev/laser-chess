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

        public Mesh mesh;
    }

    [System.Serializable]
    public class PieceGridBehaviour
    {
        public bool canLeap;
        public bool isInfinite;
        public List<Vector2Int> directionsOrPositions;
    }
}