using BoardGame.Game;
using System.Collections.Generic;
using UnityEngine;
namespace BoardGame.Config
{
    public class MovePieceBehaviourNode : DecoratorNode
    {
        /*
         * Por questão de simplicidade  deixei 
         * MovePieceBehaviourNode herdar de DecoratorNode
         * Pois nesse momento MovePieceBehaviourNode aceita apenas
         * um objeto filho. No futuro, com a possibilidade de ramificações 
         * o uso e implementação da CompositeNode pode ser mais indicado.
         */

        public MoveDirection _moveDirection;

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override IEnumerable<NodeResult> OnUpdate(Tile currentPosition)
        {
            Vector2 nextTilePos = GetPos(_moveDirection, currentPosition.GetCoordinates());
            Tile nextTile = GameManager.Instance.GetTile(nextTilePos);

            if (nextTile == null)
            {
                yield return new NodeResult(NodeBehaviour.Failure, this);
                yield break;
            }

            if (nextTile._occupiedBy != null)
            {
                yield return new NodeResult(NodeBehaviour.Failure, this);
                yield break;
            }

            if (_child == null)
            {
                yield return new NodeResult(NodeBehaviour.Success, this);
                yield break;
            }
            else
            {
                yield return new NodeResult(NodeBehaviour.Success, this);
            }

            Node child = _child;

            foreach(NodeResult node in child.UpdateNode(nextTile))
            {
                yield return node;
            }
        }

        private Vector2 GetPos(MoveDirection direction, Vector2 tilePos)
        {
            switch (direction)
            {
                case MoveDirection.Up:
                    return new Vector2(tilePos.x - 1, tilePos.y);
                case MoveDirection.Down:
                    return new Vector2(tilePos.x + 1, tilePos.y);
                case MoveDirection.Right:
                    return new Vector2(tilePos.x, tilePos.y + 1);
                case MoveDirection.Left:
                    return new Vector2(tilePos.x, tilePos.y - 1);
                case MoveDirection.UpperRight:
                    return new Vector2(tilePos.x - 1, tilePos.y + 1);
                case MoveDirection.DownRight:
                    return new Vector2(tilePos.x + 1, tilePos.y + 1);
                case MoveDirection.UpperLeft:
                    return new Vector2(tilePos.x - 1, tilePos.y - 1);
                case MoveDirection.DownLeft:
                    return new Vector2(tilePos.x + 1, tilePos.y - 1);
                default:
                    return Vector2.zero;
            }
        }
    }
}




