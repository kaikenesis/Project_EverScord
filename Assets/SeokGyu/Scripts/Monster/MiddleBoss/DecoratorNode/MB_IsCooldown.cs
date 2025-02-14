using System;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_DecoratorNode/IsCooldown", fileName = "IsCooldown")]
    public class MB_IsCooldown : DecoratorNode
    {
        public override void Init()
        {
            onUpdate = IsCooldown;
            if (child == null) return;

            child.Init();
        }

        private INode.ENodeState IsCooldown()
        {
            Debug.Log("MiddleBoss_IsCooldown");
            return INode.ENodeState.SUCCESS;
        }
    }
}
