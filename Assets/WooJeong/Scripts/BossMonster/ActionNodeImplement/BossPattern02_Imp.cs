using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern02_Imp : AttackPatternImplement
{
    protected override IEnumerator Action()
    {
        Debug.Log("Attack2 start");
        yield return new WaitForSeconds(5f);
        isEnd = true;
        action = null;
        Debug.Log("Attack2 end");
    }
}
