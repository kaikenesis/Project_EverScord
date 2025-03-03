using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NMM2_Controller : NController
{
    protected override void Setup()
    {
        monsterType = MonsterType.MEDIUM;
        GameObject projector = new GameObject();
        projector.name = "projector1";
        projector.transform.parent = transform;
        Projector1 = projector.AddComponent<DecalProjector>();
        Projector1.renderingLayerMask = 2;
        Projector1.material = ResourceManager.Instance.GetAsset<Material>("DecalRedCircle");
        Projector1.size = new Vector3(monsterData.AttackRangeX1,
                              monsterData.AttackRangeZ1,
                              monsterData.AttackRangeY1);
        Projector1.pivot = new Vector3(0, 2.5f, 0);
        Projector1.enabled = false;
        projector.transform.Rotate(90, 0, 0);
        runState = gameObject.AddComponent<NMM2_RunState>();
        attackState1 = gameObject.AddComponent<NMM2_AttackState1>();
        attackState2 = gameObject.AddComponent<NMM2_AttackState2>();
        waitState = gameObject.AddComponent<NMM2_WaitState>();
        stunState = gameObject.AddComponent<NMM2_StunState>();
        deathState = gameObject.AddComponent<NMM2_DeathState>();
    }
}
