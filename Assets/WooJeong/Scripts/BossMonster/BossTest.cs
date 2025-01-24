using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTest : MonoBehaviour
{
    [SerializeField] private BossData bossData;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            bossData.ReduceHp(10);
    }
}
