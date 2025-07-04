using EverScord;
using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern13_Imp : BossPattern06_Imp
{
    protected override void Awake()
    {
        base.Awake();
        safeRange = 5f;
        safeScale = 0.5f;
        failurePhase = 1;
        damage = bossRPC.BossMonsterData.SkillDatas[11].SkillDotDamage;
    }
}
