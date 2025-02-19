using System;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_ActionNode/Pattern3", fileName = "Pattern3")]
    public class MB_PatternNode3 : ActionNode
    {
        private MB_Controller owner;

        public override void Init()
        {
            onUpdate = Pattern3;
        }

        private INode.ENodeState Pattern3()
        {
            int num;
            blackboard.GetValue("PatternNum", out num);
            if (num != 2) return INode.ENodeState.FAILURE;

            blackboard.GetValue("Owner", out owner);
            if (owner is IAction action)
            {
                action.DoAction(IAction.EType.Action3);
            }
            blackboard.SetValue("bCooldown", true);

            Debug.Log("MiddleBoss_Pattern3");
            return INode.ENodeState.SUCCESS;
        }
    }
}
