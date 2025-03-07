using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDebuffSystem : MonoBehaviour
{
    private Dictionary<BossDebuff, BaseBossDebuff> debuffDict = new();

    public Action<BossDebuff> OnBossDebuffStart;
    public Action<BossDebuff> OnBossDebuffEnd;

    public void SubcribeOnBossDebuffStart(Action<BossDebuff> action)
    {
        OnBossDebuffStart += action;
    }

    public void SubcribeOnBossDebuffEnd(Action<BossDebuff> action)
    {
        OnBossDebuffEnd += action;
    }

    public void ClearActions()
    {
        foreach(BossDebuff d in Enum.GetValues(typeof(BossDebuff)))
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

        debuffDict[BossDebuff.POISON] = new BossDebuffPoison();
        debuffDict[BossDebuff.SLOW] = new BossDebuffSlow();
    }

    public void SetDebuff(BossRPC boss, BossDebuff bossDebuff, float time, float value)
    {
        if (debuffDict.Count == 0)
            Initialize();

        StartCoroutine(debuffDict[bossDebuff].StartDebuff(boss, time, value));
        StartCoroutine(DebuffEnd(bossDebuff, time));
        OnBossDebuffStart?.Invoke(bossDebuff);
    }

    private IEnumerator DebuffEnd(BossDebuff bossDebuff, float time)
    {
        yield return new WaitForSeconds(time);
        OnBossDebuffEnd?.Invoke(bossDebuff);
    }
}
