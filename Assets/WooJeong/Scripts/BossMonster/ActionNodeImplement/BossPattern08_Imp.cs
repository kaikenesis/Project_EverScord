using EverScord;
using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern08_Imp : ActionNodeImplement
{
    protected int attackCount = 30;

    protected override IEnumerator Act()
    {
        StartCoroutine(nameof(CheckDeath));
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
