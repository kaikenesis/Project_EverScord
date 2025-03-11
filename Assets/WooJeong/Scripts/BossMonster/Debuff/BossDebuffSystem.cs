using EverScord.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDebuffSystem : MonoBehaviour
{
    private Dictionary<EBossDebuff, BaseBossDebuff> debuffDict = new();
    public Action<EBossDebuff> OnBossDebuffStart;
    public Action<EBossDebuff> OnBossDebuffEnd;

    public void SubcribeOnBossDebuffStart(Action<EBossDebuff> action)
    {
        OnBossDebuffStart += action;
    }

    public void SubcribeOnBossDebuffEnd(Action<EBossDebuff> action)
    {
        OnBossDebuffEnd += action;
    }

    public void ClearActions()
    {
        foreach(EBossDebuff d in Enum.GetValues(typeof(EBossDebuff)))
        {
            OnBossDebuffEnd?.Invoke(d);
        }
        OnBossDebuffStart = null;
        OnBossDebuffEnd = null;
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (debuffDict.Count > 0)
            return;

        debuffDict[EBossDebuff.POISON] = new BossDebuffPoison();
        debuffDict[EBossDebuff.SLOW] = new BossDebuffSlow();
    }

    public void SetDebuff(BossRPC boss, EBossDebuff bossDebuff, CharacterControl attacker, float time, float value)
    {
        if (debuffDict.Count == 0)
            Initialize();

        StartCoroutine(debuffDict[bossDebuff].StartDebuff(boss, attacker, time, value));
        StartCoroutine(DebuffEnd(bossDebuff, time));
        OnBossDebuffStart?.Invoke(bossDebuff);
    }

    private IEnumerator DebuffEnd(EBossDebuff bossDebuff, float time)
    {
        yield return new WaitForSeconds(time);
        OnBossDebuffEnd?.Invoke(bossDebuff);
    }
}
