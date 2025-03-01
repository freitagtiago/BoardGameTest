using BoardGame.Config;
using System.Collections.Generic;

namespace BoardGame.Game
{
    public class PlayerPiece : Piece
    {
        public List<BehaviourTreeSO> _behaviourTrees = new List<BehaviourTreeSO>();
        private BoardHandler _boardHandler;
    
        void Awake()
        {
            _boardHandler = FindObjectOfType<BoardHandler>();
        }
    
        public void DrawMovementChoices()
        {
            _boardHandler.ResetHighlights(); 
    
            for(int i = 0; i < _behaviourTrees.Count; i++)
            {
                _boardHandler.EvaluateMoves(_behaviourTrees[i], _currentTile, i, this);
            }
        }
    
        public void ApplyMovement(int behaviourTreeIndex)
        {
            _boardHandler.ResetHighlights();
            _boardHandler.ApplyMovement(this, behaviourTreeIndex);
        }
    }
}