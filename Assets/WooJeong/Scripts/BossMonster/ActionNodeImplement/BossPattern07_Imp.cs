using EverScord;
using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern07_Imp : ActionNodeImplement
{
    private float attackDamage = 10;
    //private float attackRadius = 100;
    private float attackLifeTime = 4;
    private float safeRange = 7.5f;
    private float safeScale = 0.7f;
    private float curTime = 0;
    private float attackSpan = 0.1f;
    private Vector3 safePos = Vector3.zero;
    private float safeStartDistance = 4;
    private float randomRange = 10;

    protected override IEnumerator Act()
    {
        Debug.Log("Attack7 start");
        bossRPC.PlayAnimation("StandingAttack");
        safePos = transform.position + transform.forward * safeStartDistance;
        bossRPC.SetPositionScaleP7_SafeZone(safePos, safeScale);
        bossRPC.SetActivePattern7(true);
        yield return new WaitForSeconds(1f);
        StartCoroutine(MoveSafePos(attackLifeTime));   
        StartCoroutine(Attack(attackLifeTime));
        yield return new WaitForSeconds(bossRPC.clipDict["StandingAttack"] - 1f);
        bossRPC.PlayAnimation("Idle");
        yield return new WaitForSeconds(4f);
        bossRPC.SetActivePattern7(false);
        Debug.Log("Attack7 end");
        curTime = 0;
        isEnd = true;
        action = null;
        yield return null;
    }

    private IEnumerator Attack(float time)
    {
        while (true)
        {
            curTime += attackSpan;
            if(curTime >= time)
            {
                curTime = 0;
                yield break;
            }

            foreach (var player in GameManager.Instance.playerPhotonViews)
            {
                float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
                float distanceToSafe = Vector3.Distance(player.transform.position, safePos);
                Debug.Log(distanceToSafe);
                if (distanceToSafe > safeRange/2)
                {
                    CharacterControl control = player.GetComponent<CharacterControl>();
                    control.DecreaseHP(attackDamage);
                    Debug.Log("p7 hit");
                }
            }
            //bossRPC.ScalingProjectorP7_Danger();
            yield return new WaitForSeconds(attackSpan);
        }
    }

    private IEnumerator MoveSafePos(float duration)
    {
        Vector3 startPoint = safePos;
        Vector3 endPoint = new Vector3(safePos.x + Random.Range(-randomRange, randomRange), 
            safePos.y, safePos.z + Random.Range(-randomRange, randomRange));

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            safePos = Vector3.Lerp(startPoint, endPoint, t / duration);
            bossRPC.MoveP7_SafeZone(safePos);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
