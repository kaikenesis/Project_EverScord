using System;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_ActionNode/Pattern1", fileName = "Pattern1")]
    public class MB_PatternNode1 : ActionNode
    {
        public override void Init()
        {
            onUpdate = Pattern1;
        }

        private INode.ENodeState Pattern1()
        {
            Debug.Log("MiddleBoss_Pattern1");
            return INode.ENodeState.SUCCESS;
        }
    }
}
