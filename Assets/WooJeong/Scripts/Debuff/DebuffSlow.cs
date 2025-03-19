using EverScord.Character;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DebuffSlow : BaseDebuff
{
    public override IEnumerator StartDebuff(IEnemy enemy, CharacterControl attacker, float time, float value)
    {
        Debug.Log("boss slow");
        float timer = time;
        NavMeshAgent agent = enemy.GetNavMeshAgent();
        agent.speed -= value;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
            if(timer <= 0)
            {
                agent.speed += value;
                yield break;
            }
        }
    }
}
