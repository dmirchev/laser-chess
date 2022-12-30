using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    [System.Serializable]
    public class LevelGrid
    {
        [Header("Grid")]
        [SerializeField] private int gridSize;
        public int GridSize { get { return gridSize; } }
        private int[,] grid;

        [Header("Models")]
        [SerializeField] public BoxCollider gridBoxCollider;

        void Awake()
        {
            grid = new int[gridSize, gridSize];
        }

        public void SetGridCell(int x, int y, int index)
        {
            grid[x, y] = index;
        }

        public bool CheckGridCell(int x, int y)
        {
            return grid[x, y] > -1;
        }
    }
}