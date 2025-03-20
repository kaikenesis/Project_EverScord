using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackNodeImplement : ActionNodeImplement
{
    protected float attackableHP = 100;
    protected bool attackable = false;

    protected override void Awake()
    {
        attackable = false;
        base.Awake();
    }

    public override NodeState Evaluate()
    {
        if (attackable == false && bossRPC.IsUnderHP(attackableHP))
        {
            attackable = true;
            action = StartCoroutine(Act());
        }

        if (attackable == false)
        {
            return NodeState.FAILURE;
        }

        if (isEnd == false && action == null)
        {
            int rand = Random.Range(1, 9);
            if(rand != 1)
                return NodeState.FAILURE;
        }

        return base.Evaluate();
    }
}
