using BoardGame.Config;
using System.Collections.Generic;
using UnityEngine;
using Utils;
namespace BoardGame.Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private PlayerPiece _playerPiecePrefab;
        [SerializeField] private EnemyPiece _enemyPiecePrefab;
        [SerializeField] private Obstacle _obstaclePiecePrefab;

        [Header("Scriptable Objects")]
        [SerializeField] private GameConfig _boardConfig;

        [Header("Board")]
        [SerializeField] private Material _paintedMaterial;
        [SerializeField] private Material _unpaintedMaterial;
        private Tile[,] _instantiatedTilesList;

        private void Start()
        {
            //Ler config de jogo
            //Criar tabuleiro com o número de casas indicada
            CreateBoard();

            //Criar as peças e aleatorizá-las sobre as posições
            SpawnBoardPieces();
        }
        private void CreateBoard()
        {
            _instantiatedTilesList = new Tile[(int)_boardConfig._boardDimension.x
                                            , (int)_boardConfig._boardDimension.y];
            for (int i = 0; i < _boardConfig._boardDimension.x; i++)
            {
                for (int j = 0; j < _boardConfig._boardDimension.y; j++)
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
            for(int i = 0; i < _boardConfig._obstaclePiecesNumber; i++)
            {
                Tile tileSelected = GetAvailableTile();
                Obstacle obstacle = Instantiate(_obstaclePiecePrefab, tileSelected.transform);

                tileSelected.SetPiece(obstacle.gameObject);
            }

            for (int i = 0; i < _boardConfig._enemyPiecesNumber; i++)
            {
                Tile tileSelected = GetAvailableTile();
                EnemyPiece enemy = Instantiate(_enemyPiecePrefab, tileSelected.transform);

                tileSelected.SetPiece(enemy.gameObject);
            }

            for (int i = 0; i < _boardConfig._playerPiecesNumber; i++)
            {
                Tile tileSelected = GetAvailableTile();
                PlayerPiece player = Instantiate(_playerPiecePrefab, tileSelected.transform);

                tileSelected.SetPiece(player.gameObject);
            }
        }

        private Tile GetAvailableTile()
        {
            Tile tile = null;
            bool positionDefined = false;

            while (!positionDefined)
            {
                Vector2 randomPos = Utils.Utils.GetRandomVector2((int)_boardConfig._boardDimension.x
                                                                , (int)_boardConfig._boardDimension.y);

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
    }
}

