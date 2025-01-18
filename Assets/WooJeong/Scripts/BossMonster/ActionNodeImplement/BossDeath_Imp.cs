using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeath_Imp : ActionNodeImplement
{
    protected override IEnumerator Action()
    {
        Debug.Log("Death start");
        yield return new WaitForSeconds(5f);
        isEnd = true;
        action = null;
        Debug.Log("Death end");
    }
}
