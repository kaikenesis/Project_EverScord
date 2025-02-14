using System;

namespace EverScord
{
    public abstract class ActionNode : BaseNode
    {
        protected Func<INode.ENodeState> onUpdate = null;

        public override void SetBlackboard(BaseBlackBoard blackboard)
        {
            this.blackboard = blackboard;
        }

        public override INode.ENodeState Evalute()
        {
            return onUpdate?.Invoke() ?? INode.ENodeState.FAILURE;
        }
    }
}

