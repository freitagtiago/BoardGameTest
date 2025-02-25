using BoardGame.Config;
using UnityEngine;

public class TriggerNode : Node
{
    [HideInInspector] public Node _child;
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override NodeBehaviour OnUpdate()
    {
        return _child.Update();
    }

    public override Node Clone()
    {
        TriggerNode node = Instantiate(this);
        node._child = _child.Clone();
        return node;
    }
}
