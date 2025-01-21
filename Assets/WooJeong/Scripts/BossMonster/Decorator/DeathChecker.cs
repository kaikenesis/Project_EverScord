using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Decorator/DeathChecker")]
public class DeathChecker : BDecoratorNode
{
    public override NodeState Evaluate()
    {
        if(bossData.Hp > 0)
            return NodeState.SUCCESS;

        state = children[0].Evaluate();
        if(state == NodeState.SUCCESS)
            return NodeState.FAILURE;
        return state;
    }
}
