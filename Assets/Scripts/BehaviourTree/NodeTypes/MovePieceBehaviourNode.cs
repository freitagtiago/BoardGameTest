using BoardGame.Game;
using Unity.VisualScripting;
using UnityEngine;
namespace BoardGame.Config
{
    public class MovePieceBehaviourNode : CompositeNode
    {
        private int _currentNode;
        public MoveDirection _moveDirection;

        protected override void OnStart()
        {
            Debug.Log("Starting to move to " + _moveDirection.ToString());
        }

        protected override void OnStop()
        {
            Debug.Log("Stop to move to " + _moveDirection.ToString());
        }

        protected override NodeBehaviour OnUpdate()
        {
            Node child = _children[_currentNode];
            switch (child.Update())
            {
                case NodeBehaviour.Running:
                    return NodeBehaviour.Running;
                case NodeBehaviour.Failure:
                    return NodeBehaviour.Failure;
                case NodeBehaviour.Success:
                    _currentNode++;
                    break;
            }

            return _currentNode == _children.Count - 1 ? NodeBehaviour.Success : NodeBehaviour.Running;
        }
    }
}




