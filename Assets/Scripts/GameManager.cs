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
        public bool _isSelectionEnabled { get; private set; } = true;
        public PlayerPiece _selectedPiece { get; private set; } = null;

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
            _instantiatedTilesList = new Tile[_boardConfigSO._boardDimensionX
                                            , _boardConfigSO._boardDimensionY];
            for (int i = 0; i < _boardConfigSO._boardDimensionX; i++)
            {
                for (int j = 0; j < _boardConfigSO._boardDimensionY; j++)
                {
                    Tile tileInstantiated = Instantiate(_tilePrefab, this.transform);
                    tileInstantiated.Setup(new Vector2(i, j)
                                            , (i + j) % 2 == 0 ? _paintedMaterial : _unpaintedMaterial);
                    _instantiatedTilesList[i,j] = tileInstantiated;
                }
            }
        }

        private bool CanPlacePiecesOnBord()
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
            if (!CanPlacePiecesOnBord())
            {
                Debug.LogError("Há um número maior de peças do que posições. Ajuste o arquivo de configuração vinculado ao GameManager.");
                return;
            }

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
                tileSelected.SetPiece(player);
            }
        }

        private Tile GetAvailableTile()
        {
            Tile tile = null;
            bool positionDefined = false;

            while (!positionDefined)
            {
                Vector2 randomPos = Utils.Utils.GetRandomVector2(_boardConfigSO._boardDimensionX
                                                                ,_boardConfigSO._boardDimensionY);

                Tile tileCandidate = _instantiatedTilesList[(int)randomPos.x, (int)randomPos.y];
                if (tileCandidate._occupiedBy == null)
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
                || tilePos.x >= _boardConfigSO._boardDimensionX
                || tilePos.y < 0
                || tilePos.y >= _boardConfigSO._boardDimensionY)
            {
                return null;
            }
            return _instantiatedTilesList[(int)tilePos.x, (int)tilePos.y];
        }

        public void SetSelectedPiece(PlayerPiece piece)
        {
            _selectedPiece = piece;
            _isSelectionEnabled = _selectedPiece == null ? true : false;
        }
    }
}

