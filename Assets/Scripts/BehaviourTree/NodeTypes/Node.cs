using UnityEngine;

namespace BoardGame.Config
{
    public abstract class Node : ScriptableObject
    {
        [HideInInspector] public NodeBehaviour _state = NodeBehaviour.Running;
        [HideInInspector] public bool _started = false;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 _position;
        
        public NodeBehaviour Update()
        {
            if (!_started)
            {
                OnStart();
                _started = true;
            }

            _state = OnUpdate();

            if(_state == NodeBehaviour.Failure
                || _state == NodeBehaviour.Success)
            {
                OnStop();
                _started = false;
            }

            return _state;
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract NodeBehaviour OnUpdate();
    }
}

