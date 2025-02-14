using System;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_ActionNode/Wait", fileName = "Wait")]
    public class MB_Wait : ActionNode
    {
        public override void Init()
        {
            onUpdate = Wait;
        }

        private INode.ENodeState Wait()
        {
            Debug.Log("MiddleBoss_Wait");
            return INode.ENodeState.SUCCESS;
        }
    }

}
