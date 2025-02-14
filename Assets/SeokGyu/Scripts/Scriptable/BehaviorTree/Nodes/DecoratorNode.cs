using System;
using UnityEngine;

namespace EverScord
{
    public abstract class DecoratorNode : BaseNode
    {
        [SerializeField] protected BaseNode child;
        protected Func<INode.ENodeState> onUpdate = null;

        public override void SetBlackboard(BaseBlackBoard blackboard)
        {
            this.blackboard = blackboard;
            child.SetBlackboard(blackboard);
        }

        public override INode.ENodeState Evalute()
        {
            if (child == null)
                return INode.ENodeState.FAILURE;

            if(onUpdate?.Invoke() == INode.ENodeState.SUCCESS)
            {
                switch (child.Evalute())
                {
                    case INode.ENodeState.RUNNING:
                        return INode.ENodeState.RUNNING;
                    case INode.ENodeState.SUCCESS:
                        return INode.ENodeState.SUCCESS;
                }
            }

            return INode.ENodeState.FAILURE;
        }
    }
}
