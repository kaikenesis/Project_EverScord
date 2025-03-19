using EverScord;
using EverScord.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffSystem : MonoBehaviour
{
    private Dictionary<EBossDebuff, BaseDebuff> debuffDict = new();
    public static Action<EBossDebuff> OnBossDebuffStart;
    public static Action<EBossDebuff> OnBossDebuffEnd;

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
        GameManager.Instance.InitControl(this);
    }

    private void Initialize()
    {
        if (debuffDict.Count > 0)
            return;

        debuffDict[EBossDebuff.POISON] = new DebuffPoison();
        debuffDict[EBossDebuff.SLOW] = new DebuffSlow();
    }

    public void SetDebuff(IEnemy enemy, EBossDebuff debuffType, CharacterControl attacker, float time, float value)
    {
        if (debuffDict.Count == 0)
            Initialize();

        StartCoroutine(debuffDict[debuffType].StartDebuff(enemy, attacker, time, value));
        if (enemy is BossRPC == false)
            return;
        StartCoroutine(DebuffEnd(debuffType, time));
        OnBossDebuffStart?.Invoke(debuffType);
    }

    private IEnumerator DebuffEnd(EBossDebuff debuffType, float time)
    {
        yield return new WaitForSeconds(time);
        OnBossDebuffEnd?.Invoke(debuffType);
    }
}
