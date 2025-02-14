using UnityEngine;

namespace EverScord
{
    public abstract class BaseNode : ScriptableObject, INode
    {
        protected BaseBlackBoard blackboard;

        public abstract void Init();
        public abstract void SetBlackboard(BaseBlackBoard blackboard);
        public abstract INode.ENodeState Evalute();
    }
}
