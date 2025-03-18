using EverScord;
using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern08_Imp : AttackNodeImplement
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
        bossRPC.PlayAnimation("Shoot");
        yield return new WaitForSeconds(0.5f);
        bossRPC.PlaySound("BossPattern08");
        List<CharacterControl> controls = new List<CharacterControl>();
        foreach(var player in GameManager.Instance.PlayerDict.Values)
        {
            if(player.IsDead) continue;
            controls.Add(player);
        }
        int randInt = Random.Range(0, controls.Count);
        controls[randInt].ApplyDebuff(CharState.STUNNED, attackCount);
        yield return new WaitForSeconds(bossRPC.clipDict["Shoot"] - 0.5f);
        isEnd = true;
        action = null;
        yield return null;
    }
}
