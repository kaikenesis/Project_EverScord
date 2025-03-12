using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Decorator/DeathChecker")]
public class DeathChecker : BDecoratorNode
{
    public override NodeState Evaluate()
    {
        if(bossRPC.HP > 0)
            return NodeState.SUCCESS;

        state = children[0].Evaluate();
        return state;
    }
}
