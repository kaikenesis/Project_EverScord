using EverScord;
using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern09_Imp : AttackNodeImplement
{
    protected int attackCount = 30;
    protected int failurePhase = 2;

    protected override void Awake()
    {
        base.Awake();
        attackableHP = 40;
    }

    public override NodeState Evaluate()
    {
        if (bossRPC.Phase == failurePhase)
            return NodeState.FAILURE;
        return base.Evaluate();
    }

    protected override IEnumerator Act()
    {
        List<CharacterControl> controls = new List<CharacterControl>();
        foreach(var player in GameManager.Instance.PlayerDict.Values)
        {
            if(player.IsDead) continue;
            controls.Add(player);
        }
        int randInt = Random.Range(0, controls.Count);
        controls[randInt].ApplyDebuff(CharState.STUNNED, attackCount);
        bossRPC.PlayAnimation("Shoot");
        yield return new WaitForSeconds(bossRPC.clipDict["Shoot"]);
        isEnd = true;
        action = null;
        yield return null;
    }
}
