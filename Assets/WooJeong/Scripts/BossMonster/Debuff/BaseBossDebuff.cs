using EverScord.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossDebuff
{
    POISON, SLOW
}

public abstract class BaseBossDebuff
{
    protected float timer = 3f;

    public abstract IEnumerator StartDebuff(BossRPC boss, CharacterControl attacker, float time, float value);        
}
