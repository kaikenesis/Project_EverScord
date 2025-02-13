using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern07_Imp : ActionNodeImplement
{
    protected override IEnumerator Act()
    {
        isEnd = true;
        action = null;
        yield return null;
    }

}
