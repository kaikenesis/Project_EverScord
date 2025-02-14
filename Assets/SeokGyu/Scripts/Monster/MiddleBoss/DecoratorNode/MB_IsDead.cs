using System;
using Unity.VisualScripting;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_DecoratorNode/IsDead", fileName = "IsDead")]
    public class MB_IsDead : DecoratorNode
    {
        public override void Init()
        {
            onUpdate = IsDead;
            if (child == null) return;

            child.Init();
        }

        private INode.ENodeState IsDead()
        {
            bool bDeath = false;
            blackboard.GetValue("IsDead", out bDeath);

            if (bDeath == false)
                return INode.ENodeState.FAILURE;

            Debug.Log("MiddleBoss_IsDead");
            return INode.ENodeState.SUCCESS;
        }
    }
}
