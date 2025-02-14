using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/AI/BehaviorTree/SelectNode", fileName = "newSelectNode")]
    public class SelectNode : BaseNode
    {
        [SerializeField] private List<BaseNode> childs;

        public SelectNode(List<BaseNode> childs)
        {
            this.childs = childs;
        }

        public override void Init()
        {
            int count = childs.Count;

            for (int i = 0; i < count; i++)
            {
                childs[i].Init();
            }
        }

        public override void SetBlackboard(BaseBlackBoard blackboard)
        {
            this.blackboard = blackboard;
            int count = childs.Count;

            for (int i = 0; i < count; i++)
            {
                childs[i].SetBlackboard(blackboard);
            }
        }

        public override INode.ENodeState Evalute()
        {
            if (childs == null)
                return INode.ENodeState.FAILURE;

            for (int i = 0; i < childs.Count; i++)
            {
                switch (childs[i].Evalute())
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
