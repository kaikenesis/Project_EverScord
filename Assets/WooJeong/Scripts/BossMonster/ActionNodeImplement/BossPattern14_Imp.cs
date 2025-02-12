using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern14_Imp : ActionNodeImplement
{
    protected override IEnumerator Act()
    {
        Debug.Log("Attack14 start");
        yield return new WaitForSeconds(5f);
        isEnd = true;
        action = null;
        Debug.Log("Attack14 end");
    }
}
