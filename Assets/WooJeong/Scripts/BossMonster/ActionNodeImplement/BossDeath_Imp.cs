using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeath_Imp : ActionNodeImplement
{
    protected override IEnumerator Act()
    {
        Debug.Log("Death start");
        bossRPC.PlayAnimation("Die");
        yield return new WaitForSeconds(2f);
        isEnd = true;
        action = null;
        //ResourceManager.Instance.ReturnToPool(gameObject, "Boss");
        Debug.Log("Death end");
    }
}
