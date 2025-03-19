using EverScord;
using EverScord.Character;
using System.Collections;
using UnityEngine;

public class DebuffPoison : BaseDebuff
{
    public override IEnumerator StartDebuff(IEnemy enemy, CharacterControl attacker, float time, float tickDamage)
    {
        float timer = time;
        while (true)
        {
            yield return new WaitForSeconds(1f);
            GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(attacker, tickDamage, enemy, false);
            timer--;
            if (timer <= 0)
                yield break;
        }
    }
}
