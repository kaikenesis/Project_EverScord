using System;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_ActionNode/Pattern2", fileName = "Pattern2")]
    public class MB_PatternNode2 : ActionNode
    {
        private MB_Controller owner;

        public override void Init()
        {
            onUpdate = Pattern2;
        }

        private INode.ENodeState Pattern2()
        {
            int num;
            blackboard.GetValue("PatternNum", out num);
            if (num != 1) return INode.ENodeState.FAILURE;

            blackboard.GetValue("Owner", out owner);
            if (owner is IAction action)
            {
                action.DoAction(IAction.EType.Action2);
            }
            blackboard.SetValue("bCooldown", true);

            Debug.Log("MiddleBoss_Pattern2");
            return INode.ENodeState.SUCCESS;
        }
    }
}
