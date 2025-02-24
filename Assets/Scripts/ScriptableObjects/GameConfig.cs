using UnityEngine;

namespace BoardGame.Config
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
    public class GameConfig : ScriptableObject
    {
        public Vector2 _boardDimension = new Vector2(8, 8);
        public int _playerPiecesNumber = 4;
        public int _enemyPiecesNumber = 10;
        public int _obstaclePiecesNumber = 8;
    }
}

