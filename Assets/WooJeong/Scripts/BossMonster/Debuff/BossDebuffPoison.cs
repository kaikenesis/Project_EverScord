using System.Collections;
using UnityEngine;

public class BossDebuffPoison : BaseBossDebuff
{
    public override IEnumerator StartDebuff(BossRPC boss, float time, float tickDamage)
    {
        timer = time;
        while (true)
        {
            boss.DecreaseHP(tickDamage);
            yield return new WaitForSeconds(1f);
            timer--;
            if (timer <= 0)
                yield break;
        }
    }
}
