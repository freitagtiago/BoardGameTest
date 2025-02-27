using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using BoardGame.Config;
using System.Linq;
using System.Collections.Generic;

public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }
    private BehaviourTreeSO _tree;

    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());
        focusable = true;
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }

    private NodeView FindNodeView(BoardGame.Config.Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    internal void PopulateView(BehaviourTreeSO tree)
    {
        this._tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if(_tree._rootNode == null)
        {
            _tree._rootNode = _tree.CreateNode(typeof(TriggerNode)) as TriggerNode;
            EditorUtility.SetDirty(_tree);
            AssetDatabase.SaveAssets();
        }

        _tree._nodes.ForEach(n => CreateNodeView(n));

        _tree._nodes.ForEach(n =>{
            var children = _tree.GetChildren(n);
            children.ForEach(c =>
            {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);

                if(childView != null)
                {
                    if(childView._input != null)
                    {
                        Edge edge = parentView._output.ConnectTo(childView._input);
                        AddElement(edge);
                    }
                }
            });
        });
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                NodeView nodeView = elem as NodeView;
                if (nodeView != null)
                {
                    _tree.DeleteNode(nodeView._node);
                }

                Edge edge = elem as Edge;
                if(edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;

                    _tree.RemoveChild(parentView._node, childView._node);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;

                _tree.AddChild(parentView._node, childView._node);
            });
        }

        EditorUtility.SetDirty(_tree);
        AssetDatabase.SaveAssets();

        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        /*
         *  Adicionar aqui novas classes de comportamento
         *  para criar o submenu da ferramenta de edição
         */

        {
            var actionNodeTypes = TypeCache.GetTypesDerivedFrom<ActionNode>();

            foreach (var type in actionNodeTypes)
            {
                string menuPath = $"Add/Behaviour/{type.Name}";

                evt.menu.AppendAction(menuPath, (a) => CreateNode(type));
            }
        }

        {
            var decoratorNodeTypes = TypeCache.GetTypesDerivedFrom<DecoratorNode>();

            foreach (var type in decoratorNodeTypes)
            {
                string menuPath = $"Add/Behaviour/{type.Name}";

                evt.menu.AppendAction(menuPath, (a) => CreateNode(type));
            }
        }

        {
            var composideNodeTypes = TypeCache.GetTypesDerivedFrom<CompositeNode>();

            foreach (var type in composideNodeTypes)
            {
                string menuPath = $"Add/Behaviour/{type.Name}";

                evt.menu.AppendAction(menuPath, (a) => CreateNode(type));
            }
        }

    }

    public override EventPropagation DeleteSelection()
    {
        return base.DeleteSelection();
    }

    private void CreateNode(System.Type type)
    {
        BoardGame.Config.Node node = _tree.CreateNode(type);
        CreateNodeView(node);
    }

    private void CreateNodeView(BoardGame.Config.Node node)
    {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;

        if(_tree._nodes.Count > 0)
        {
            int lastNodeIndex = _tree._nodes.Count - 2;
            if (lastNodeIndex < 0)
            {
                AddElement(nodeView);
                return;
            }

            Vector2 newNodePosition = new Vector2(200, 200);
            float maxX = float.MinValue;

            BoardGame.Config.Node lastNode = _tree._nodes[lastNodeIndex];

            if(node._position == Vector2.zero
                && nodeView._input != null)
            {
                float nodeRightEdge = lastNode._position.x + 200;
                if (nodeRightEdge > maxX)
                {
                    maxX = nodeRightEdge;
                    newNodePosition = new Vector2(maxX + 50, lastNode._position.y);
                }

                nodeView.SetPosition(new Rect(newNodePosition, new Vector2(150, 100)));

                if (nodeView._input != null)
                {
                    if (lastNode is DecoratorNode)
                    {
                        (lastNode as DecoratorNode)._child = node;
                        NodeView lastNodeView = FindNodeView(lastNode);

                        Edge edge = lastNodeView._output.ConnectTo(nodeView._input);
                        AddElement(edge);
                    }
                    else if (lastNode is TriggerNode)
                    {
                        (lastNode as TriggerNode)._child = node;
                        NodeView lastNodeView = FindNodeView(lastNode);

                        Edge edge = lastNodeView._output.ConnectTo(nodeView._input);
                        AddElement(edge);
                    }
                }
            }
            else
            {
                nodeView.SetPosition(new Rect(node._position, new Vector2(150, 100)));
            }
        }
        AddElement(nodeView);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

}
