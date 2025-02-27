using UnityEngine;
using System.Collections.Generic;
using BoardGame.Config;
using System.Linq;
using System.Collections;
namespace BoardGame.Game
{
    public class BoardHandler : MonoBehaviour
    {
        public BehaviourTreeSO movementTree;
        public Tile[,] boardTiles;

        private List<Tile> _highlithedTiles = new List<Tile>();

        public void EvaluateMoves(BehaviourTreeSO tree, Tile currentPosition, int behaviourTreeIndex)
        {
            movementTree = tree;

            if (movementTree == null 
                || currentPosition == null)
            {
                Debug.LogError("BehaviourTreeSO ou tile inicial não definidos");
                return;
            }

            movementTree.StartExecution(currentPosition);
            StartCoroutine(DrawPathsFromTree(currentPosition, behaviourTreeIndex));
        }

        private IEnumerator<NodeBehaviour> DrawPathsFromTree(Tile currentPosition, int behaviourTreeIndex)
        {
            List<Tile> validMoves = new List<Tile>();

            foreach (NodeResult state in movementTree.UpdateTree(currentPosition))
            {
                if (state._state == NodeBehaviour.Success)
                {
                    Tile validTile = GetTileFromBehaviour(currentPosition, state._node);
                    if (validTile != null)
                    {
                        _highlithedTiles.Add(validTile);
                        validMoves.Add(validTile);
                        validTile.HandlePathIndicator(true, false, behaviourTreeIndex);
                        currentPosition = validTile;
                    }
                }else if (state._state == NodeBehaviour.Failure)
                {
                    Tile validTile = GetTileFromBehaviour(currentPosition, state._node);
                    if (validTile != null
                        && validTile._occupiedBy is EnemyPiece)
                    {
                        _highlithedTiles.Add(validTile);
                        validMoves.Add(validTile);
                        validTile.HandlePathIndicator(true, true, behaviourTreeIndex);
                    }
                }
                yield return state._state;
            }

            if (validMoves.Count > 0)
            {
                (validMoves.LastOrDefault()).HandlePathIndicator(true, true, behaviourTreeIndex);
            }

            validMoves.Clear();
            GameManager.Instance._isSelectionEnabled = !(_highlithedTiles.Count > 0);
        }

        private IEnumerator MovePieceFromTree(PlayerPiece playerPiece, int treeBehaviourIndex, Tile currentPosition)
        {
            movementTree = playerPiece._behaviourTrees[treeBehaviourIndex];
            movementTree.StartExecution(currentPosition);
            foreach (NodeResult state in movementTree.UpdateTree(currentPosition))
            {
                if (state._state == NodeBehaviour.Success)
                {
                    Tile validTile = GetTileFromBehaviour(currentPosition, state._node);
                    if (validTile != null)
                    {
                        currentPosition.SetPiece(null);
                        validTile.SetPiece(playerPiece);
                        playerPiece.transform.parent = validTile.transform;
                        playerPiece.transform.localPosition = Vector3.zero;

                        yield return new WaitForSecondsRealtime(1f);

                        currentPosition = validTile;
                    }
                }
                else if (state._state == NodeBehaviour.Failure)
                {
                    Tile validTile = GetTileFromBehaviour(currentPosition, state._node);
                    if (validTile != null
                        && validTile._occupiedBy is EnemyPiece)
                    {
                        currentPosition.SetPiece(playerPiece);
                        currentPosition = validTile;
                        playerPiece.transform.parent = validTile.transform;
                        playerPiece.transform.localPosition = Vector3.zero;

                        yield return new WaitForSecondsRealtime(1f);
                    }
                }
                currentPosition.SetPiece(playerPiece);
            }

            GameManager.Instance._selectedPiece = null;
            GameManager.Instance._isSelectionEnabled = true;
        }

        private Tile GetTileFromBehaviour(Tile currentPosition, Node node)
        {
            MovePieceBehaviourNode move = node as MovePieceBehaviourNode;

            return GameManager.Instance.GetTile(GetPos(move._moveDirection, currentPosition.GetCoordinates()));

        }

        public void ResetHighlights()
        {
            foreach (Tile tile in _highlithedTiles)
            {
                tile.HandlePathIndicator(false, false, -1);
            }
            _highlithedTiles.Clear();
        }

        public void ApplyMovement(PlayerPiece playerPiece, int treeBehaviourIndex)
        {
            StartCoroutine(MovePieceFromTree(playerPiece, treeBehaviourIndex, playerPiece.transform.parent.GetComponent<Tile>()));
        }

        private Vector2 GetPos(MoveDirection direction, Vector2 tilePos)
        {
            switch (direction)
            {
                case MoveDirection.Up:
                    return new Vector2(tilePos.x - 1, tilePos.y);
                case MoveDirection.Down:
                    return new Vector2(tilePos.x + 1, tilePos.y);
                case MoveDirection.Right:
                    return new Vector2(tilePos.x, tilePos.y + 1);
                case MoveDirection.Left:
                    return new Vector2(tilePos.x, tilePos.y - 1);
                case MoveDirection.UpperRight:
                    return new Vector2(tilePos.x - 1, tilePos.y + 1);
                case MoveDirection.DownRight:
                    return new Vector2(tilePos.x + 1, tilePos.y + 1);
                case MoveDirection.UpperLeft:
                    return new Vector2(tilePos.x - 1, tilePos.y - 1);
                case MoveDirection.DownLeft:
                    return new Vector2(tilePos.x + 1, tilePos.y - 1);
                default:
                    return Vector2.zero;
            }
        }
    }
}

