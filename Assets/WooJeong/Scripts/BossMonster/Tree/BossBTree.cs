using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class BossBTree : BTree
{
    [SerializeField] private BossData bossData;

    protected override void SetupTree()
    {
        root.Init();
        root.CreateBlackboard();
        root.SetValue("BossData", bossData);
        root.Setup(gameObject);
    }
}
