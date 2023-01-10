using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private LevelGrid levelGrid;

        [SerializeField] private Piece[] piecesPrefabs;

        [SerializeField] private Dictionary<int,Piece> pieces;

        [SerializeField] private Transform piecesParent;

        [SerializeField] private Piece selectedPiece;
        public Piece SelectedPiece { get { return selectedPiece; } }

        public void Init()
        {
            levelGrid.CreateLevelGrid(this);

            LevelData levelData = LevelDataManager.GetLevelData();

            pieces = new Dictionary<int, Piece>();

            Piece piecePrefab;
            Piece piece;
            int index = 0;
            for (int i = 0; i < levelData.pieces.Length; i++)
            {
                piecePrefab = piecesPrefabs[levelData.pieces[i].index];

                for (int j = 0; j < levelData.pieces[i].positions.Length; j++)
                {
                    piecePrefab = piecesPrefabs[levelData.pieces[i].index];

                    piece = Instantiate(
                        piecesPrefabs[levelData.pieces[i].index], 
                        levelGrid.GetCellWorldCenter(levelData.pieces[i].positions[j]),
                        piecePrefab.pieceInfo.playerControlled ? Quaternion.identity : Quaternion.AngleAxis(180.0f, Vector3.up), 
                        piecesParent
                    );

                    piece.cellCoordinates = levelData.pieces[i].positions[j];

                    levelGrid.SetGridIndex(piece.cellCoordinates, index);
                    pieces.Add(index, piece);

                    index++;
                }
            }

            ClearSelectedPiece();
        }

        public bool CheckIsPlayerFinished(out bool hasPlayerWon)
        {
            int playerCount = 0;
            int aiCount = 0;
            hasPlayerWon = false;
            foreach(KeyValuePair<int, Piece> piece in pieces)
            {
                if (piece.Value.pieceInfo.playerControlled)
                    playerCount++;
                else
                    aiCount++;
            }

            if (playerCount == 0 || aiCount == 0)
            {
                DeleteAll();

                hasPlayerWon = aiCount == 0;

                return true;
            }

            foreach(KeyValuePair<int, Piece> piece in pieces)
            {
                if (piece.Value.pieceInfo.playerControlled == GameplayGameState.isPlayer && 
                    piece.Value.PieceStateType == PieceStateType.Idle)
                    return false;
            }

            GameplayGameState.isPlayer = !GameplayGameState.isPlayer;

            foreach(KeyValuePair<int, Piece> piece in pieces)
            {
                if (piece.Value.pieceInfo.playerControlled == GameplayGameState.isPlayer)
                    piece.Value.Reset();
            }

            return false;
        }

        void DeleteAll()
        {
            foreach(KeyValuePair<int, Piece> piece in pieces)
            {
                Destroy(piece.Value.gameObject);
            }
        }

        public void ClearSelectedPiece()
        {
            selectedPiece = null;
            levelGrid.ClearCells();
        }

        public bool HasSelectedPiece()
        {
            return selectedPiece != null;
        }

        public void SelectOwnPiece(RaycastHit hit, bool isAPiece)
        {
            Piece piece = SelectPiece(hit, isAPiece);

            if (piece != null && 
                piece.pieceInfo.playerControlled == GameplayGameState.isPlayer && 
                piece.PieceStateType == PieceStateType.Idle
            )
            {
                selectedPiece = piece;

                levelGrid.ShowSquares(
                    selectedPiece.pieceInfo.movePieceGridBehaviour, 
                    selectedPiece.cellCoordinates,
                    GetLevelDirection(selectedPiece.pieceInfo.playerControlled),
                    true
                );
            }
        }

        public bool HasSelectedSamePiece(RaycastHit hit, bool isAPiece)
        {
            Piece piece = SelectPiece(hit, isAPiece);

            return piece != null && piece.Equals(selectedPiece);
        }

        public bool IsCellAvailableToMove(RaycastHit hit, bool isAPiece)
        {
            if (isAPiece) return false;

            Vector3 cellCenter;
            Vector2Int cellCoordinates;
            if (levelGrid.IsCellAvailable(hit.point, selectedPiece.cellCoordinates, out cellCenter, out cellCoordinates))
            {
                selectedPiece.MoveAgent(cellCenter, cellCoordinates);
                return true;
            }

            return false;
        }

        public bool IsCellAvailableToAttackOnce(RaycastHit hit, bool isAPiece)
        {
            int index;
            if (levelGrid.IsCellNotAvailable(hit.point, out index))
            {
                if (index == -1) return false;
                if (!AreOpponents(index)) return false;

                selectedPiece.Attack();

                if (pieces[index].TakeDamage(selectedPiece.pieceInfo.attackPower))
                {
                    levelGrid.ClearGridIndex(pieces[index].cellCoordinates);
                    pieces.Remove(index);
                }

                return true;
            }

            return false;
        }

        public void AreCellAvailableToAttackAll()
        {
            List<int> indices = levelGrid.AreCellNotAvailable();
            int index;
            selectedPiece.Attack();
            for (int i = 0; i < indices.Count; i++)
            {
                index = indices[i];

                if (index == -1) continue;
                if (!AreOpponents(index)) continue;

                if (pieces[index].TakeDamage(selectedPiece.pieceInfo.attackPower))
                {
                    levelGrid.ClearGridIndex(pieces[index].cellCoordinates);
                    pieces.Remove(index);
                }
            }
        }

        public void AttackNone()
        {
            selectedPiece.Attack();
        }

        private Piece SelectPiece(RaycastHit hit, bool isAPiece)
        {
            if (isAPiece)
            {
                return hit.collider.GetComponent<Piece>();
            }
            else
            {
                int index = levelGrid.GetGridIndex(hit.point);
                if (index > 0)
                    return pieces[index];
            }

            return null;
        }

        public void ShowSquaresAttack()
        {
            levelGrid.ShowSquares(
                selectedPiece.pieceInfo.attackPieceGridBehaviour, 
                selectedPiece.cellCoordinates,
                GetLevelDirection(selectedPiece.pieceInfo.playerControlled),
                false
            );
        }

        Vector3 GetLevelDirection(bool isPlayer)
        {
            return isPlayer ? Vector3.forward : Vector3.back;
        }

        public bool AreOpponents(int index)
        {
            return selectedPiece.pieceInfo.playerControlled != pieces[index].pieceInfo.playerControlled;
        }

        void OnDrawGizmosSelected()
        {
            levelGrid.OnDrawGizmosSelected();
        }

        /* List<Piece> GetCurrentPlayerPieces()
        {
            return GameplayGameState.isPlayer ? playerPieces : aiPieces;
        }

        List<Piece> GetOpponentPlayerPieces()
        {
            return GameplayGameState.isPlayer ? aiPieces : playerPieces;
        } */
    }
}