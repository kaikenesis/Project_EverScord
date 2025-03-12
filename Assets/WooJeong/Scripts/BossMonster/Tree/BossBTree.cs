using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class BossBTree : BTree
{
    private BossRPC bossRPC;

    protected override void SetupTree()
    {
        bossRPC = GetComponent<BossRPC>();
        root.Init();
        root.CreateBlackboard();
        root.SetValue("BossRPC", bossRPC);
        root.Setup(gameObject);
    }
}
