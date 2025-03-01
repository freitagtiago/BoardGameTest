using UnityEngine;
using System.Collections.Generic;
using BoardGame.Config;
using System.Linq;
using System.Collections;
using Unity.VisualScripting;
using static UnityEditor.Experimental.GraphView.GraphView;
using System;
namespace BoardGame.Game
{
    public class BoardHandler : MonoBehaviour
    {
        public static BoardHandler Instance;

        [Header("Prefabs")]
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private List<PlayerPiece> _playerPiecePrefab = new List<PlayerPiece>();
        [SerializeField] private EnemyPiece _enemyPiecePrefab;
        [SerializeField] private Obstacle _obstaclePiecePrefab;

        [Header("Scriptable Objects")]
        [SerializeField] private GameConfigSO _boardConfigSO;

        [Header("Board")]
        [SerializeField] private Material _paintedMaterial;
        [SerializeField] private Material _unpaintedMaterial;
        private Tile[,] _instantiatedTilesMatrix;
        private List<Tile> _highlithedTiles = new List<Tile>();

        public BehaviourTreeSO currentMovementTree;
        List<Tile> availableTiles = new List<Tile>();

        public Action<bool> OnEndDrawing;
        public Action<Tile> OnEndBehaviour;

        #region Board Creation
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            CreateBoard();
            SpawnBoardPieces();
        }
        private void CreateBoard()
        {
            _instantiatedTilesMatrix = new Tile[_boardConfigSO._boardDimensionX
                                            , _boardConfigSO._boardDimensionY];
            for (int i = 0; i < _boardConfigSO._boardDimensionX; i++)
            {
                for (int j = 0; j < _boardConfigSO._boardDimensionY; j++)
                {
                    Tile tileInstantiated = Instantiate(_tilePrefab, this.transform);
                    tileInstantiated.Setup(new Vector2(i, j)
                                            , (i + j) % 2 == 0 ? _paintedMaterial : _unpaintedMaterial);
                    _instantiatedTilesMatrix[i, j] = tileInstantiated;
                }
            }

            availableTiles = _instantiatedTilesMatrix.Cast<Tile>().ToList();
        }

        private bool CanPlacePiecesOnBoard()
        {
            int tileNumber = _boardConfigSO._boardDimensionX * _boardConfigSO._boardDimensionY;
            int piecesToSet = _boardConfigSO._playerPiecesNumber + _boardConfigSO._enemyPiecesNumber + _boardConfigSO._obstaclePiecesNumber;
            if (tileNumber < piecesToSet)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void SpawnBoardPieces()
        {
            if (!CanPlacePiecesOnBoard())
            {
                Debug.LogError("Há um número maior de peças do que posições. Ajuste o arquivo de configuração vinculado ao GameManager.");
                return;
            }

            for (int i = 0; i < _boardConfigSO._obstaclePiecesNumber; i++)
            {
                Tile tileSelected = GetAvailableTile();
                Obstacle obstacle = Instantiate(_obstaclePiecePrefab, tileSelected.transform);
                obstacle._currentTile = tileSelected;

                tileSelected.SetPiece(obstacle);
            }

            for (int i = 0; i < _boardConfigSO._enemyPiecesNumber; i++)
            {
                Tile tileSelected = GetAvailableTile();
                EnemyPiece enemy = Instantiate(_enemyPiecePrefab, tileSelected.transform);
                enemy._currentTile = tileSelected;
                tileSelected.SetPiece(enemy);
            }

            /*
             * Foram vinculados 4 tipos de prefabs para player no editor
             * No futuro essa lógica deve ser diferente para contemplar melhor uma lógica de jogo
             * Por hora pra evitar estouros, caso o número de playerPieces seja maior que o número de prefabs vinculados
             * apenas repito o primeiro prefab.
             */
            for (int i = 0; i < _boardConfigSO._playerPiecesNumber; i++)
            {
                Tile tileSelected = GetAvailableTile();

                PlayerPiece player;
                if (_playerPiecePrefab.Count < i)
                {
                    player = Instantiate(_playerPiecePrefab[0], tileSelected.transform);
                }
                else
                {
                    player = Instantiate(_playerPiecePrefab[i], tileSelected.transform);
                }
                player._currentTile = tileSelected;
                tileSelected.SetPiece(player);
            }

            Debug.Log("Peças foram distribuídas, selecione sua peça");
        }

        private Tile GetAvailableTile()
        {
            Tile tile = availableTiles[(UnityEngine.Random.Range(0,availableTiles.Count - 1))];
            availableTiles.Remove(tile);
            return tile;
        }
        #endregion

        #region Navigation Methods
        public void EvaluateMoves(BehaviourTreeSO tree, Tile currentPosition, int behaviourTreeIndex, Piece actingPiece)
        {
            currentMovementTree = tree;

            if (currentMovementTree == null 
                || currentPosition == null)
            {
                Debug.LogError("BehaviourTreeSO ou tile inicial não definidos");
                return;
            }

            currentMovementTree.StartExecution(currentPosition, actingPiece);
            StartCoroutine(DrawPathsFromTree(currentPosition, behaviourTreeIndex, actingPiece));
        }
        /*
         * As sequências de caminhos poderiam ser salvas em uma lista para que não seja necessário 
         * percorrer novamente a lista para desempenhar o movimento. Como entendi que o foco do teste era
         * trabalhar com a ferramenta de grafo, repeti a lógica para percorrer a lista ao invés de fazer um cache dos movimentos.
         */
        private IEnumerator<NodeBehaviour> DrawPathsFromTree(Tile currentPosition, int behaviourTreeIndex, Piece actingPiece)
        {
            List<Tile> validMoves = new List<Tile>();

            foreach (NodeResult state in currentMovementTree.UpdateTree(currentPosition, actingPiece))
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
            if(_highlithedTiles.Count > 0)
            {
                OnEndDrawing?.Invoke(false);
            }
        }

        private IEnumerator MovePieceFromTree(PlayerPiece playerPiece, int treeBehaviourIndex, Tile currentPosition)
        {
            currentMovementTree = playerPiece._behaviourTrees[treeBehaviourIndex];
            currentMovementTree.StartExecution(currentPosition, playerPiece);
            foreach (NodeResult state in currentMovementTree.UpdateTree(currentPosition, playerPiece))
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
            OnEndBehaviour?.Invoke(currentPosition);
        }

        private Tile GetTileFromBehaviour(Tile currentPosition, Node node)
        {
            MovePieceBehaviourNode move = node as MovePieceBehaviourNode;

            return GetTile(GetPos(move._moveDirection, currentPosition.GetCoordinates()));

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
        public Tile GetTile(Vector2 tilePos)
        {
            if (tilePos.x < 0
                || tilePos.x >= _boardConfigSO._boardDimensionX
                || tilePos.y < 0
                || tilePos.y >= _boardConfigSO._boardDimensionY)
            {
                return null;
            }
            return _instantiatedTilesMatrix[(int)tilePos.x, (int)tilePos.y];
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
        #endregion
    }
}

