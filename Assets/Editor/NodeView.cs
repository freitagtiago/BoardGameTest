using UnityEngine;
using BoardGame.Config;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using BoardGame.Game;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public BoardGame.Config.Node _node;
    public Port _input;
    public Port _output;

    public NodeView(BoardGame.Config.Node node)
    {
        this._node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        style.left = node._position.x;
        style.top = node._position.y;

        CreateInputPort();
        CreateOutputPort();

        if(_node is MovePieceBehaviourNode)
        {
            var moveDirectionField = new EnumField("Move Direction", (_node as MovePieceBehaviourNode)._moveDirection);
            moveDirectionField.RegisterValueChangedCallback(evt =>
            {
                (_node as MovePieceBehaviourNode)._moveDirection = (MoveDirection)evt.newValue;
            });
            this.mainContainer.Add(moveDirectionField);
        }
    }

    private void CreateInputPort()
    {
        if(_node is ActionNode)
        {
            _input = InstantiatePort(Orientation.Horizontal
                                    , Direction.Input
                                    , Port.Capacity.Single
                                    , typeof(bool));
        }else if(_node is CompositeNode)
        {
            _input = InstantiatePort(Orientation.Horizontal
                                    , Direction.Input
                                    , Port.Capacity.Single
                                    , typeof(bool));
        }
        else if(_node is DecoratorNode)
        {
            _input = InstantiatePort(Orientation.Horizontal
                                    , Direction.Input
                                    , Port.Capacity.Single
                                    , typeof(bool));
        }
        else if (_node is TriggerNode)
        {

        }

        if (_input != null)
        {
            _input.portName = "";
            inputContainer.Add(_input);
        }
    }
    private void CreateOutputPort()
    {
        if (_node is ActionNode)
        {

        }
        else if (_node is CompositeNode)
        {
            _output = InstantiatePort(Orientation.Horizontal
                                    , Direction.Output
                                    , Port.Capacity.Single
                                    , typeof(bool));
        }
        else if (_node is DecoratorNode)
        {
            _output = InstantiatePort(Orientation.Horizontal
                                    , Direction.Output
                                    , Port.Capacity.Single
                                    , typeof(bool));
        }
        else if (_node is TriggerNode)
        {
            _output = InstantiatePort(Orientation.Horizontal
                                    , Direction.Output
                                    , Port.Capacity.Single
                                    , typeof(bool));
        }

        if (_output != null)
        {
            _output.portName = "";
            outputContainer.Add(_output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        _node._position.x = newPos.xMin;
        _node._position.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if(OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }
}
