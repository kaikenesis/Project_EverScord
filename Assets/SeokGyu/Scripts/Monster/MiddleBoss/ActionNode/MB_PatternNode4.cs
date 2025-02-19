using System;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_ActionNode/Pattern4", fileName = "Pattern4")]
    public class MB_PatternNode4 : ActionNode
    {
        private MB_Controller owner;

        public override void Init()
        {
            onUpdate = Pattern4;
        }

        private INode.ENodeState Pattern4()
        {
            int num;
            blackboard.GetValue("PatternNum", out num);
            if (num != 3) return INode.ENodeState.FAILURE;

            blackboard.GetValue("Owner", out owner);
            if (owner is IAction action)
            {
                action.DoAction(IAction.EType.Action4);
            }
            blackboard.SetValue("bCooldown", true);

            Debug.Log("MiddleBoss_Pattern4");
            return INode.ENodeState.SUCCESS;
        }

    }
}
