using BoardGame.Game;
using System.Collections.Generic;
using UnityEngine;
namespace BoardGame.Config
{
    public class TriggerNode : Node
    {
        [HideInInspector] public Node _child;
        protected override void OnStart()
        {
            
        }
    
        protected override void OnStop()
        {
            
        }
    
        protected override IEnumerable<NodeResult> OnUpdate(Tile currentTile)
        {
            return _child.UpdateNode(currentTile);
        }
    
        public override Node Clone()
        {
            TriggerNode node = Instantiate(this);
            node._child = _child.Clone();
            return node;
        }
    }
}