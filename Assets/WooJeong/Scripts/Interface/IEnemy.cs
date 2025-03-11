using System;
using UnityEngine;
using EverScord.Effects;
using EverScord.Character;

public interface IEnemy
{
    public void TestDamage(GameObject sender, float value);
    public void DecreaseHP(float hp, CharacterControl attacker);
    public void StunMonster(float stunTime);
    public BlinkEffect GetBlinkEffect();
}
