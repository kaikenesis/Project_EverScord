using EverScord.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBossDebuff
{
    POISON, SLOW
}

public abstract class BaseDebuff
{
    public abstract IEnumerator StartDebuff(IEnemy enemy, CharacterControl attacker, float time, float value);
}
