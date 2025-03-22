using EverScord;
using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern06_Imp : AttackNodeImplement
{
    //private float attackRadius = 100;
    private float attackLifeTime = 4;
    protected float safeRange = 7.5f;
    protected float safeScale = 0.7f;
    private float curTime = 0;
    private float attackSpan = 0.5f;
    private Vector3 safePos = Vector3.zero;
    private float safeStartDistance = 4;
    private float randomRange = 10;
    protected int failurePhase = 2;
    protected float damage;
    private Dictionary<CharacterControl, float> hitPlayers = new();

    protected override void Awake()
    {
        base.Awake();
        attackableHP = 60;
        damage = bossRPC.BossMonsterData.SkillDatas[5].SkillDotDamage;
    }

    public override NodeState Evaluate()
    {
        if (bossRPC.Phase == failurePhase)
            return NodeState.FAILURE;
        return base.Evaluate();
    }

    protected override IEnumerator Act()
    {
        Debug.Log("Attack6 start");
        bossRPC.PlayAnimation("StandingAttack");
        bossRPC.PlaySound("BossPattern06");
        safePos = transform.position + transform.forward * safeStartDistance;
        bossRPC.SetPositionScaleP6_SafeZone(safePos, safeScale);
        bossRPC.SetActivePattern6(true);
        yield return new WaitForSeconds(1f);
        Vector3 endPoint = new Vector3(safePos.x + Random.Range(-randomRange, randomRange),
            safePos.y, safePos.z + Random.Range(-randomRange, randomRange));
        bossRPC.MoveP6_SafeZone(attackLifeTime, endPoint);
        StartCoroutine(Attack(attackLifeTime));
        yield return new WaitForSeconds(bossRPC.clipDict["StandingAttack"] - 1f);

        bossRPC.PlayAnimation("Idle");
        yield return new WaitForSeconds(4f);
        bossRPC.SetActivePattern6(false);

        Debug.Log("Attack6 end");
        bossRPC.StopSound("BossPattern06");
        curTime = 0;
        isEnd = true;
        action = null;
        yield return null;
    }

    private IEnumerator Attack(float time)
    {
        hitPlayers.Clear();
        while (true)
        {
            curTime += Time.deltaTime;
            if(curTime >= time)
            {
                curTime = 0;
                yield break;
            }

            foreach (CharacterControl player in GameManager.Instance.PlayerDict.Values)
            {
                if(hitPlayers.ContainsKey(player))
                    hitPlayers[player] -= Time.deltaTime;

                float distanceToSafe = Vector3.Distance(player.PlayerTransform.position, bossRPC.GetSafePos());

                if (distanceToSafe > safeRange/2)
                {
                    if (hitPlayers.ContainsKey(player) == false || hitPlayers[player] <= 0)
                    {
                        hitPlayers[player] = attackSpan;
                        player.DecreaseHP(damage * 0.5f, true);
                    }
                }
            }
            yield return null;
        }
    }
}
