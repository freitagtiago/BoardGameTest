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

        protected override State OnUpdate()
        {
            Node child = _children[_currentNode];
            switch (child.Update())
            {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    return State.Failure;
                case State.Success:
                    _currentNode++;
                    break;
            }

            return _currentNode == _children.Count ? State.Success : State.Running;
        }
    }
}




