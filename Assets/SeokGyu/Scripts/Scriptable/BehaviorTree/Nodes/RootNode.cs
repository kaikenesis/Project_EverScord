using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/AI/BehaviorTree/RootNode", fileName = "newRootNode")]
    public class RootNode : BaseNode
    {
        [SerializeField] private BaseNode child;

        public RootNode(BaseNode child)
        {
            this.child = child;
        }

        public override void Init()
        {
            if (child == null) return;

            child.Init();
        }

        public override void SetBlackboard(BaseBlackBoard blackboard)
        {
            this.blackboard = blackboard;
            child.SetBlackboard(blackboard);
        }

        public override INode.ENodeState Evalute()
        {
            if (child == null)
                return INode.ENodeState.FAILURE;

            switch(child.Evalute())
            {
                case INode.ENodeState.RUNNING:
                    return INode.ENodeState.RUNNING;
                case INode.ENodeState.SUCCESS:
                    return INode.ENodeState.SUCCESS;
            }

            return INode.ENodeState.FAILURE;
        }
    }
}
