using System;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_ActionNode/Pattern1", fileName = "Pattern1")]
    public class MB_PatternNode1 : ActionNode
    {
        private MB_Controller Owner;

        public override void Init()
        {
            onUpdate = Pattern1;
        }

        private INode.ENodeState Pattern1()
        {
            Debug.Log("LagerPattern Play");

            blackboard.GetValue("Owner", out Owner);
            Owner.StartCoroutine("LagerPattern");
            blackboard.SetValue("bCooldown", true);

            return INode.ENodeState.SUCCESS;
        }
    }
}
