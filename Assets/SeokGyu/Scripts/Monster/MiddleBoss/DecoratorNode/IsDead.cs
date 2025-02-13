using UnityEngine;

namespace EverScord
{
    public class IsDead : MB_Decorator
    {
        public override NodeState Evaluate()
        {
            if (owner.CurHealth <= 0.0f)
                return NodeState.SUCCESS;

            return NodeState.FAILURE;
        }
    }
}
