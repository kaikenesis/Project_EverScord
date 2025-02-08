using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class BossBTree : BTree
{
    [SerializeField] private BossData bossData;
    private Animator animator;

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (root != null)
            root.Evaluate();
    }

    protected override void SetupTree()
    {
        animator = GetComponent<Animator>();
        root.Init();
        root.CreateBlackboard();
        bossData.ResetParams();
        root.SetValue("BossData", bossData);
        root.SetValue("Animator", animator);
        root.Setup(gameObject);
    }
}
