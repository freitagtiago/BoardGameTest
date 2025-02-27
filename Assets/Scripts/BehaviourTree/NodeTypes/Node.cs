using BoardGame.Game;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame.Config
{
    public abstract class Node : ScriptableObject
    {
        [HideInInspector] public NodeBehaviour _state = NodeBehaviour.Running;
        [HideInInspector] public bool _started = false;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 _position;
        
        public IEnumerable<NodeResult> UpdateNode(Tile currentPosition)
        {
            if (!_started)
            {
                OnStart();
                _started = true;
            }

            foreach (var result in OnUpdate(currentPosition))
            {
                _state = result._state;
                yield return result; 
            }

            if (_state == NodeBehaviour.Failure
                || _state == NodeBehaviour.Success)
            {
                OnStop();
                _started = false;
            }
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract IEnumerable<NodeResult> OnUpdate(Tile currentPosition);
    }
}

