using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTest : MonoBehaviour
{
    [SerializeField] private BossData bossData;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            bossData.ReduceHp(10);
    }
}
