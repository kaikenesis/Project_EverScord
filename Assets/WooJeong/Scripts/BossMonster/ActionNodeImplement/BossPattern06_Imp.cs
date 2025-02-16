using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern06_Imp : ActionNodeImplement
{
    private float laserLifeTime = 3;

    protected override IEnumerator Act()
    {
        Debug.Log("Attack6 start");
        bossRPC.PlayAnimation("RotatingShot");
        yield return new WaitForSeconds(1f);
        yield return bossRPC.LaserEnable(laserLifeTime);
        bossRPC.PlayAnimation("Idle");
        isEnd = true;
        action = null;
        Debug.Log("Attack6 end");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("laser hit");
            CharacterControl control = other.GetComponent<CharacterControl>();
            control.DecreaseHP(10);
        }
    }
}
