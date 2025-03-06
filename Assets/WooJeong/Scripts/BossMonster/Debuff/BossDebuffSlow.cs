using System.Collections;
using UnityEngine;

public class BossDebuffSlow : BaseBossDebuff
{
    public override IEnumerator StartDebuff(BossRPC boss, float time, float value)
    {
        Debug.Log("boss slow");
        timer = time;
        boss.BossNavMeshAgent.speed -= value;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
            if(timer <= 0)
            {
                boss.BossNavMeshAgent.speed += value;
                yield break;
            }
        }
    }
}
