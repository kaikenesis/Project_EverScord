using System;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MB_ActionNode/Wait", fileName = "Wait")]
    public class MB_Wait : ActionNode
    {
        float countCooldown = 0.0f;
        float cooldown;

        public override void Init()
        {
            onUpdate = Wait;
        }

        private INode.ENodeState Wait()
        {
            blackboard.GetValue("Cooldown", out cooldown);

            if(countCooldown >= cooldown)
            {
                blackboard.SetValue("bCooldown", false);
                countCooldown = 0.0f;
            }
            else
                countCooldown += Time.deltaTime;

            //Debug.Log($"count : {countCooldown}");

            return INode.ENodeState.SUCCESS;
        }
    }

}
