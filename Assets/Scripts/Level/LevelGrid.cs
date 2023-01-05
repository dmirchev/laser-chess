using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace LaserChess
{
    [System.Serializable]
    public class LevelGrid
    {
        private Level level;

        [Header("Grid")]
        [SerializeField] private int gridSize;
        [HideInInspector] public int GridSize { get { return gridSize; } }
        [SerializeField] private int cellSize;
        [HideInInspector] public int CellSize { get { return cellSize; } }
        private int[,] grid;
        private Vector2Int selectedCell;
        [SerializeField] private List<Vector2Int> availableCells;
        [SerializeField] private List<Vector2Int> displayCells;

        [Header("Models")]
        [SerializeField] private GameObject gridCellPrefab;
        [SerializeField] private Transform gridParent;
        [SerializeField] private Transform gridCellsParent;
        [SerializeField] private BoxCollider gridBoxCollider;

        [Header("Elements")]
        private MeshRenderer[,] gridCellsMeshRenderers;
        [SerializeField] private NavMeshSurface navMeshSurface;

        [Header("Colors")]
        [SerializeField] private Color normalColor;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color moveColor;
        [SerializeField] private Color attackColor;
        [SerializeField] private Color damageColor;
        private MaterialPropertyBlock materialPropertyBlock;

        public void CreateLevelGrid(Level level)
        {
            this.level = level;

            int length = gridSize * gridSize;

            grid = new int[gridSize, gridSize];
            for(int i = 0; i < grid.GetLength(0); i++)  { 
                for(int j = 0; j < grid.GetLength(1); j++)  {
                    grid[i, j] = -1;
                }
            }

            availableCells = new List<Vector2Int>(length);
            displayCells = new List<Vector2Int>(length);

            Vector3 gridScale = Vector3.up + (new Vector3(1, 0, 1) * CellSize);
            gridParent.localScale = gridScale;

            gridCellsMeshRenderers = new MeshRenderer[gridSize, gridSize];

            materialPropertyBlock = new MaterialPropertyBlock();

            GameObject cellGameObject;
            Vector3 cellPosition = Vector3.zero;

            int x,y;
            for (int i = 0; i < length; i++)
            {
                x = i % gridSize;
                y = i / gridSize;
                cellPosition.x = x * cellSize - gridSize;
                cellPosition.z = y * cellSize - gridSize;
                cellGameObject = MonoBehaviour.Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, gridCellsParent);
                gridCellsMeshRenderers[x, y] = cellGameObject.GetComponent<MeshRenderer>();
                SetCellColor(x, y, normalColor);
            }

            navMeshSurface.BuildNavMesh();
        }

        public Vector2Int GetGridCoordinates(Vector3 poisition)
        {
            poisition /= cellSize;
            Vector2 roundedPoint = new Vector2(Mathf.FloorToInt(poisition.x), Mathf.FloorToInt(poisition.z));
            roundedPoint += Vector2.one * gridSize * 0.5f;

            return new Vector2Int((int)roundedPoint.x, (int)roundedPoint.y);
        }

        public Vector3 GetCellWorldCenter(Vector2Int cellCoordinates)
        {
            Vector2 floatPoint = new Vector2(cellCoordinates.x, cellCoordinates.y);
            floatPoint -= Vector2.one * gridSize * 0.5f;
            floatPoint *= cellSize;
            floatPoint += Vector2.one * cellSize * 0.5f;

            return new Vector3(floatPoint.x, 0, floatPoint.y);
        }

        public int GetGridIndex(Vector3 poisition)
        {
            Vector2Int vInt = GetGridCoordinates(poisition);
            return GetGridCell(vInt.x, vInt.y);
        }

        public void SetGridIndex(Vector2Int coordinates, int index)
        {
            SetGridCell(coordinates.x, coordinates.y, index);
        }

        public void ClearGridIndex(Vector2Int coordinates)
        {
            SetGridCell(coordinates.x, coordinates.y, -1);
        }

        public bool IsCellAvailable(Vector3 poisition, Vector2Int oldCellCoordinates, out Vector3 cellCenter, out Vector2Int cellCoordinates)
        {
            cellCenter = Vector3.zero;
            cellCoordinates = oldCellCoordinates;
            Vector2Int vInt = GetGridCoordinates(poisition);

            if (CheckGridCell(vInt.x, vInt.y))
                return false;
            
            bool isAvailable = availableCells.Contains(vInt);
            if (isAvailable)
            {
                cellCenter = GetCellWorldCenter(vInt);
                cellCenter.y = poisition.y;
                cellCoordinates = vInt;

                ReorderGrid(oldCellCoordinates, cellCoordinates);
            }
            
            return isAvailable;
        }

        public bool IsCellNotAvailable(Vector3 poisition, out int index)
        {
            index = -1;
            Vector2Int vInt = GetGridCoordinates(poisition);

            if (!CheckGridCell(vInt.x, vInt.y))
                return false;
            
            bool isAvailable = availableCells.Contains(vInt);

            if (isAvailable) index = GetGridCell(vInt.x, vInt.y);
            
            return isAvailable;
        }

        public List<int> AreCellNotAvailable()
        {
            List<int> indices = new List<int>();

            Vector2Int vInt;
            for (int i = 0; i < availableCells.Count; i++)
            {
                vInt = availableCells[i];

                if (!CheckGridCell(vInt.x, vInt.y))
                    continue;
            
                if (availableCells.Contains(vInt)) indices.Add(GetGridCell(vInt.x, vInt.y));
            }

            return indices;
        }

        private void SetGridCell(int x, int y, int index)
        {
            grid[x, y] = index;
        }

        private int GetGridCell(int x, int y)
        {
            return grid[x, y];
        }

        private bool CheckGridCell(int x, int y)
        {
            return grid[x, y] > -1;
        }

        private int GetAndRemoveGridCell(int x, int y)
        {
            int index = GetGridCell(x, y);
            SetGridCell(x, y, -1);
            return index;
        }

        private void ReorderGrid(Vector2Int oldCellCoordinates, Vector2Int newCellCoordinates)
        {
            SetGridCell(
                newCellCoordinates.x,
                newCellCoordinates.y,
                GetAndRemoveGridCell(oldCellCoordinates.x, oldCellCoordinates.y)
            );
        }

        private List<Vector2Int> RotateDirections(List<Vector2Int> directionsOrPositions, Vector3 worldDirection)
        {
            List<Vector2Int> rotatedDirectionsOrPositions = new List<Vector2Int>(directionsOrPositions);

            float angle = Mathf.Atan2(worldDirection.x, worldDirection.z) * Mathf.Rad2Deg;
            Quaternion worldRotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 rotated;
            for (int i = 0; i < rotatedDirectionsOrPositions.Count; i++)
            {
                rotated = new Vector3(rotatedDirectionsOrPositions[i].x, 0, rotatedDirectionsOrPositions[i].y);
                rotated = worldRotation * rotated;

                rotatedDirectionsOrPositions[i] = new Vector2Int((int)rotated.x, (int)rotated.z);
            }

            return rotatedDirectionsOrPositions;
        }

        public void ShowSquares(PieceGridBehaviour pieceGridBehaviour, Vector2Int center, Vector3 worldDirection, bool isMove)
        {
            ClearCells();

            selectedCell = center;
            SetCellColor(center.x, center.y, selectedColor);

            List<Vector2Int> rotatedDirectionsOrPositions = RotateDirections(pieceGridBehaviour.directionsOrPositions, worldDirection);

            Vector2Int directionOrPosition;
            Vector2Int offset = Vector2Int.one * gridSize;
            bool notAvailable;
            for (int i = 0; i < rotatedDirectionsOrPositions.Count; i++)
            {
                notAvailable = false;
                if (pieceGridBehaviour.canLeap)
                {
                    directionOrPosition = center + rotatedDirectionsOrPositions[i];
                    AddToAvailableCellsToMove(directionOrPosition, isMove, out notAvailable);
                }
                else
                {
                    directionOrPosition = center;

                    if (pieceGridBehaviour.isInfinite)
                    {
                        while (!notAvailable)
                        {
                            directionOrPosition += rotatedDirectionsOrPositions[i];
                            AddToAvailableCellsToMove(directionOrPosition, isMove, out notAvailable);
                        }
                    }
                    else
                    {
                        int length = GetRoundedMagnitude(rotatedDirectionsOrPositions[i]);
                        Vector2Int direction = GetRoundedDirection(rotatedDirectionsOrPositions[i]);

                        for (int j = 0; j < length; j++)
                        {
                            directionOrPosition += direction;
                            AddToAvailableCellsToMove(directionOrPosition, isMove, out notAvailable);

                            if (notAvailable) break;
                        }
                    }
                }
            }
        }

        private void AddToAvailableCellsToMove(Vector2Int position, bool isMove, out bool notAvailable)
        {
            notAvailable = false;
            if (position.x < 0 || position.x > gridSize-1 || position.y < 0 || position.y > gridSize-1)
            {
                notAvailable = true;
                return;
            }

            if (CheckGridCell(position.x, position.y))
            {
                notAvailable = true;
                if (isMove) return;
            }

            if (!isMove && notAvailable)
            {
                if (!level.AreOpponents(GetGridCell(position.x, position.y)))
                    return;
            }

            if (isMove || (!isMove && notAvailable))
                availableCells.Add(position);
            else
                displayCells.Add(position);
            
            SetCellColor(position.x, position.y, GetColor(isMove, notAvailable));
        }

        Color GetColor(bool isMove, bool isOccupied)
        {
            if (isMove)
            {
                return moveColor;
            }
            else
            {
                if (isOccupied)
                    return damageColor;
                else
                    return attackColor;
            }
        }

        public void ClearCells()
        {
            SetCellColor(selectedCell.x, selectedCell.y, normalColor);

            Vector2Int position;
            for (int i = 0; i < availableCells.Count; i++)
            {
                position = availableCells[i];
                SetCellColor(position.x, position.y, normalColor);
            }

            availableCells.Clear();

            for (int i = 0; i < displayCells.Count; i++)
            {
                position = displayCells[i];
                SetCellColor(position.x, position.y, normalColor);
            }

            displayCells.Clear();
        }

        public void SetCellColor(int x, int y, Color color)
        {
            materialPropertyBlock.SetColor("_BaseColor", color);
            gridCellsMeshRenderers[x, y].SetPropertyBlock(materialPropertyBlock);
        }

        Vector2Int GetRoundedDirection(Vector2Int v)
        {
            return new Vector2Int(
                v.x == 0 ? 0 : ((int)Mathf.Sign(v.x))*v.x/v.x, 
                v.y == 0 ? 0 : ((int)Mathf.Sign(v.y))*v.y/v.y
            );
        }

        int GetRoundedMagnitude(Vector2Int v)
        {
            return Mathf.Abs(v.x == 0 ? v.y : v.x);
        }

        public void OnDrawGizmosSelected()
        {
            Vector3 startPosition = gridBoxCollider.bounds.size * -0.5f;
            startPosition.y = 0;

            Gizmos.color = Color.green;

            for (int i = 0; i < gridSize + 1; i++)
            {
                Gizmos.DrawRay(startPosition + Vector3.right * i * CellSize, Vector3.forward * gridSize * 2);
                Gizmos.DrawRay(startPosition + Vector3.forward * i * CellSize, Vector3.right * gridSize * 2);
            }
        }
    }
}