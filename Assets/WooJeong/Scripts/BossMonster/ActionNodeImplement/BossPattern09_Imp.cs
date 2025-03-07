using EverScord;
using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern09_Imp : ActionNodeImplement
{
    protected int attackCount = 30;

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
