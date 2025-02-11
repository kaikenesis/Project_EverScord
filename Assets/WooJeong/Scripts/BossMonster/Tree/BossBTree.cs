using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class BossBTree : BTree
{
    [SerializeField] private BossData bossData;

    protected override void SetupTree()
    {
        _ = ResourceManager.Instance.CreatePool("BossProjectile");
        root.Init();
        root.CreateBlackboard();
        bossData.ResetParams();
        root.SetValue("BossData", bossData);
        root.Setup(gameObject);
    }
}
