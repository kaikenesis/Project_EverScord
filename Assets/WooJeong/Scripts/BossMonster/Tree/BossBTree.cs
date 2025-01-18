using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBTree : BTree
{
    [SerializeField] private BossData bossData;
    //private NavMeshAgent navMeshAgent;

    protected override void SetupTree()
    {
        root.Init();
        root.CreateBlackboard();
        bossData.ResetParams();
        root.SetValue("BossData", bossData);
        root.Setup(gameObject);
        //navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        //root.SetValue("NavMeshAgent", navMeshAgent);
    }
}
