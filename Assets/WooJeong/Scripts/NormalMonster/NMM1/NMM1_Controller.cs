using DTT.AreaOfEffectRegions;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class NMM1_Controller : NController
{
    public GameObject LineIndicatorObject;
    public SRPLineRegionProjector LineProjector;

    protected override void Setup()
    {
        monsterType = MonsterType.MEDIUM;
        runState = gameObject.AddComponent<NMM1_RunState>();
        attackState1 = gameObject.AddComponent<NMM1_AttackState1>();
        attackState2 = gameObject.AddComponent<NMM1_AttackState2>();       
        waitState = gameObject.AddComponent<NMM1_WaitState>();
        stunState = gameObject.AddComponent<NMM1_StunState>();
        deathState = gameObject.AddComponent<NMM1_DeathState>();
    }    

    public void ProjectLineIndicator(float duration)
    {
        PhotonView.RPC(nameof(SyncProjectLineIndicator_NMM1), RpcTarget.All, duration);
    }

    [PunRPC]
    private IEnumerator SyncProjectLineIndicator_NMM1(float duration)
    {
        LineIndicatorObject.SetActive(true);
        LineProjector.FillProgress = 0;
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime;
            if (t >= duration)
            {
                LineIndicatorObject.SetActive(false);
                yield break;
            }
            LineProjector.FillProgress = t / duration;
            LineProjector.UpdateProjectors();
            yield return null;
        }
    }
}
