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
    
        public void DrawMovementChoices(Tile currentTile)
        {
            _boardHandler.ResetHighlights(); 
    
            foreach(BehaviourTreeSO tree in _behaviourTrees)
            {
                _boardHandler.EvaluateMoves(tree, currentTile, _behaviourTrees.IndexOf(tree));
            }
        }
    
        public void ApplyMovement(Tile currentTile, int behaviourTreeIndex)
        {
            _boardHandler.ResetHighlights();
            _boardHandler.ApplyMovement(this, behaviourTreeIndex);
        }
    }
}