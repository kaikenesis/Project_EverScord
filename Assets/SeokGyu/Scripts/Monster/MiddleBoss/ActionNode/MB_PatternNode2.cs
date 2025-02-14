using System;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_ActionNode/Pattern2", fileName = "Pattern2")]
    public class MB_PatternNode2 : ActionNode
    {
        public override void Init()
        {
            onUpdate = Pattern2;
        }

        private INode.ENodeState Pattern2()
        {
            Debug.Log("MiddleBoss_Pattern2");
            return INode.ENodeState.SUCCESS;
        }
    }
}
