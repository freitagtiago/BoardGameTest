using UnityEngine;

namespace BoardGame.Config
{
    [CreateAssetMenu(fileName = "GameConfigSO", menuName = "ScriptableObjects/GameConfigSO", order = 1)]
    public class GameConfigSO : ScriptableObject
    {
        public int _boardDimensionX = 8;
        public int _boardDimensionY = 8;
        public int _playerPiecesNumber = 4;
        public int _enemyPiecesNumber = 10;
        public int _obstaclePiecesNumber = 8;
    }
}

