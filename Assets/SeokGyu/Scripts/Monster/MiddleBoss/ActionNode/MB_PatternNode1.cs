using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_ActionNode/Pattern1", fileName = "Pattern1")]
    public class MB_PatternNode1 : ActionNode
    {
        private MB_Controller owner;

        public override void Init()
        {
            onUpdate = Pattern1;
        }

        private INode.ENodeState Pattern1()
        {
            blackboard.GetValue("Owner", out owner);
            if(owner is IAction action)
            {
                action.DoAction(IAction.EType.Action1);
            }
            blackboard.SetValue("bCooldown", true);

            return INode.ENodeState.SUCCESS;
        }
    }
}
