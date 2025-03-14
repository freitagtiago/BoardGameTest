using BoardGame.Config;
using UnityEngine;
using BoardGame.Game;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
namespace BoardGame.Config
{
    [CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "ScriptableObjects/BehaviourTree/BehaviourTree", order = 1)]
    public class BehaviourTreeSO : ScriptableObject
    {
        public Node _rootNode;
        public NodeBehaviour _treeState = NodeBehaviour.Running;
        public List<Node> _nodes = new List<Node>();
        private IEnumerator<NodeResult> _executionIterator;


        public void StartExecution(Tile currentPosition, Piece actingPiece)
        {
            if (_rootNode == null) return;

            _executionIterator = _rootNode.UpdateNode(currentPosition, actingPiece).GetEnumerator();
        }

        public IEnumerable<NodeResult> UpdateTree(Tile currentPosition, Piece actingPiece)
        {
            if (_executionIterator == null)
            {
                _executionIterator = _rootNode.UpdateNode(currentPosition, actingPiece).GetEnumerator();
            }
            else
            {
                while (_executionIterator.MoveNext())
                {
                    NodeResult node = _executionIterator.Current;
                    _treeState = node._state;
                    yield return node;
                }
            }
            _executionIterator = null;
        }

        public BehaviourTreeSO Clone()
        {
            BehaviourTreeSO tree = Instantiate(this);
            tree._rootNode = tree._rootNode.Clone();
            return tree;
        }
        /*
         * Adicionando trativa para n�o enviar 
         * depend�ncias do editor ao buildar.
         */
#if UNITY_EDITOR
        public Node CreateNode(System.Type type)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            _nodes.Add(node);

            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node)
        {
            _nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            TriggerNode rootNode = parent as TriggerNode;
            if (rootNode)
            {
                rootNode._child = child;
            }

            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator)
            {
                decorator._child = child;
            }

            CompositeNode compositeNode = parent as CompositeNode;
            if (compositeNode)
            {
                compositeNode._children.Add(child);
            }
        }
        public void RemoveChild(Node parent, Node child)
        {
            TriggerNode rootNode = parent as TriggerNode;
            if (rootNode)
            {
                rootNode._child = null;
            }

            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator)
            {
                decorator._child = null;
            }

            CompositeNode compositeNode = parent as CompositeNode;
            if (compositeNode)
            {
                compositeNode._children.Remove(child);
            }
        }
        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new List<Node>();

            TriggerNode rootNode = parent as TriggerNode;
            if (rootNode
                && rootNode._child != null)
            {
                children.Add(rootNode._child);
            }

            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator
                && decorator._child != null)
            {
                children.Add(decorator._child);
            }

            CompositeNode compositeNode = parent as CompositeNode;
            if (compositeNode)
            {
                return compositeNode._children;
            }

            return children;
        }
#endif
    }

}
