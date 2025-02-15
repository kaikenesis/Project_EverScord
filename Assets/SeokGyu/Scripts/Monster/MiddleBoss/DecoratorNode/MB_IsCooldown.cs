using System;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_DecoratorNode/IsCooldown", fileName = "IsCooldown")]
    public class MB_IsCooldown : DecoratorNode
    {
        bool bCooldown;

        public override void Init()
        {
            onUpdate = IsCooldown;
            if (child == null) return;

            child.Init();
        }

        private INode.ENodeState IsCooldown()
        {
            blackboard.GetValue("bCooldown", out bCooldown);

            if (bCooldown == true)
                return INode.ENodeState.FAILURE;

            return INode.ENodeState.SUCCESS;
        }
    }
}
