using EverScord.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public void TestDamage(GameObject sender, float value);
    public void DecreaseHP(float hp);
    public void StunMonster(float stunTime);
    public BlinkEffect GetBlinkEffect();
}
