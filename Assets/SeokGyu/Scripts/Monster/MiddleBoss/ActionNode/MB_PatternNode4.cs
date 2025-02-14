using System;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_ActionNode/Pattern4", fileName = "Pattern4")]
    public class MB_PatternNode4 : ActionNode
    {
        public override void Init()
        {
            onUpdate = Pattern4;
        }

        private INode.ENodeState Pattern4()
        {
            Debug.Log("MiddleBoss_Pattern4");
            return INode.ENodeState.SUCCESS;
        }

    }
}
