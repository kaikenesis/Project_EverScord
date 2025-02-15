using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public void DecreaseHP(float hp);
    public void StunMonster(float stunTime);
}
