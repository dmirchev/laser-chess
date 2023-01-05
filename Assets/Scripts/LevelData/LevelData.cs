using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    [System.Serializable]
    public class LevelData
    {
        public PiecesData[] pieces;
    }

    [System.Serializable]
    public class PiecesData
    {
        public int index;
        public Vector2Int[] positions;
    }
}