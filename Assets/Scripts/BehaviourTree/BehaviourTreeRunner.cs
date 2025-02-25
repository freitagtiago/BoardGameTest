using BoardGame.Config;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTreeSO _behaviourTreeSO;

    private void Start()
    {
        _behaviourTreeSO = _behaviourTreeSO.Clone();
    }

    private void Update()
    {
        _behaviourTreeSO.Update();
    }
}
