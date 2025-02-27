using BoardGame.Config;
using System.Collections.Generic;
using UnityEngine;
namespace BoardGame.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

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
        private Tile[,] _instantiatedTilesList;
        public bool _isSelectionEnabled = true;
        public PlayerPiece _selectedPiece = null;

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            CreateBoard();
            SpawnBoardPieces();
        }
        private void CreateBoard()
        {
            _instantiatedTilesList = new Tile[(int)_boardConfigSO._boardDimension.x
                                            , (int)_boardConfigSO._boardDimension.y];
            for (int i = 0; i < _boardConfigSO._boardDimension.x; i++)
            {
                for (int j = 0; j < _boardConfigSO._boardDimension.y; j++)
                {
                    Tile tileInstantiated = Instantiate(_tilePrefab, this.transform);
                    tileInstantiated.Setup(new Vector2(i, j)
                                            , (i + j) % 2 == 0 ? _paintedMaterial : _unpaintedMaterial);
                    _instantiatedTilesList[i,j] = tileInstantiated;
                }
            }
        }

        private void SpawnBoardPieces()
        {
            for(int i = 0; i < _boardConfigSO._obstaclePiecesNumber; i++)
            {
                Tile tileSelected = GetAvailableTile();
                Obstacle obstacle = Instantiate(_obstaclePiecePrefab, tileSelected.transform);

                tileSelected.SetPiece(obstacle);
            }

            for (int i = 0; i < _boardConfigSO._enemyPiecesNumber; i++)
            {
                Tile tileSelected = GetAvailableTile();
                EnemyPiece enemy = Instantiate(_enemyPiecePrefab, tileSelected.transform);

                tileSelected.SetPiece(enemy);
            }

            for (int i = 0; i < _boardConfigSO._playerPiecesNumber; i++)
            {
                Tile tileSelected = GetAvailableTile();
                PlayerPiece player = Instantiate(_playerPiecePrefab[i], tileSelected.transform);

                tileSelected.SetPiece(player);
            }
        }

        private Tile GetAvailableTile()
        {
            Tile tile = null;
            bool positionDefined = false;

            while (!positionDefined)
            {
                Vector2 randomPos = Utils.Utils.GetRandomVector2((int)_boardConfigSO._boardDimension.x
                                                                , (int)_boardConfigSO._boardDimension.y);

                Tile tileCandidate = _instantiatedTilesList[(int)randomPos.x, (int)randomPos.y];
                if (!tileCandidate.IsOccupied())
                {
                    tile = tileCandidate;
                    positionDefined = true;
                    break;
                }
            }
            return tile;
        }

        public Tile GetTile(Vector2 tilePos)
        {
            if(tilePos.x < 0 
                || tilePos.x >= _boardConfigSO._boardDimension.x
                || tilePos.y < 0
                || tilePos.y >= _boardConfigSO._boardDimension.y)
            {
                return null;
            }
            return _instantiatedTilesList[(int)tilePos.x, (int)tilePos.y];
        }
    }
}

