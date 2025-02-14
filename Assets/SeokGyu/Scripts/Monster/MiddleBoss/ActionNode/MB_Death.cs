using System;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_ActionNode/Death", fileName = "Death")]
    public class MB_Death : ActionNode
    {
        public override void Init()
        {
            onUpdate = Death;
        }

        private INode.ENodeState Death()
        {
            bool bDeath;
            blackboard.GetValue("IsDead", out bDeath);

            if (bDeath == false)
            {
                return INode.ENodeState.FAILURE;
            }

            Debug.Log("MiddleBoss_Death");
            return INode.ENodeState.SUCCESS;
        }
    }
}
