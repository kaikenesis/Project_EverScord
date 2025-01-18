using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BossPattern01_Imp : AttackPatternImplement
{
    protected override IEnumerator Action()
    {
        Debug.Log("Attack1 start");
        yield return new WaitForSeconds(5f);
        isEnd = true;
        action = null;
        Debug.Log("Attack1 end");
    }
}
