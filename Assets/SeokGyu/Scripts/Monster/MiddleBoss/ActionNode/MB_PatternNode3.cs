using System;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_ActionNode/Pattern3", fileName = "Pattern3")]
    public class MB_PatternNode3 : ActionNode
    {
        public override void Init()
        {
            onUpdate = Pattern3;
        }

        private INode.ENodeState Pattern3()
        {
            Debug.Log("MiddleBoss_Pattern3");
            return INode.ENodeState.SUCCESS;
        }
    }
}
