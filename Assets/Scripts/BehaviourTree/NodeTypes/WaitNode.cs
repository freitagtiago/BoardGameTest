using UnityEngine;
namespace BoardGame.Config
{
    public class WaitNode : ActionNode
    {
        public float _duration = 1f;
        private float _startTime;

        protected override void OnStart()
        {
            _startTime = Time.time;
        }

        protected override void OnStop()
        {
            
        }

        protected override NodeBehaviour OnUpdate()
        {
            if(Time.time - _startTime > _duration)
            {
                return NodeBehaviour.Success;
            }
            return NodeBehaviour.Running;
        }
    }
}

