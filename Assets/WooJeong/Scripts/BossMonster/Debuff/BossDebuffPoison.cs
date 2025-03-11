using EverScord;
using EverScord.Character;
using System.Collections;
using UnityEngine;

public class BossDebuffPoison : BaseBossDebuff
{
    public override IEnumerator StartDebuff(BossRPC boss, CharacterControl attacker, float time, float tickDamage)
    {
        timer = time;
        while (true)
        {
            //boss.DecreaseHP(tickDamage);
            GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(attacker, tickDamage, boss, false);

            yield return new WaitForSeconds(1f);
            timer--;
            if (timer <= 0)
                yield break;
        }
    }
}
