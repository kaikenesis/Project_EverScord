using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SK_112301_Controller : NController
{
    private float chargeRange;

    public float ChargeRange { get { return chargeRange; } }

    protected override void Setup()
    {
        runState = gameObject.AddComponent<SK_112301_RunState>();
        attackState1 = gameObject.AddComponent<SK_112301_AttackState1>();
        attackState2 = gameObject.AddComponent<SK_112301_AttackState2>();
        waitState = gameObject.AddComponent<SK_112301_WaitState>();
        stunState = gameObject.AddComponent<SK_112301_StunState>();
        deathState = gameObject.AddComponent<SK_112301_DeathState>();

        var temp = monsterData as NMD_112301;
        chargeRange = temp.ChargeRange;
        
        player = GameObject.Find("Player");
        Animator = GetComponentInChildren<Animator>();
        Projector1 = gameObject.AddComponent<DecalProjector>();
        Projector2 = gameObject.AddComponent<DecalProjector>();
        BoxCollider1 = gameObject.AddComponent<BoxCollider>();
        BoxCollider2 = gameObject.AddComponent<BoxCollider>();

        ProjectorSetup();
        Projector2.size = new Vector3(monsterData.AttackRangeX2,
                              monsterData.AttackRangeY2,
                              chargeRange);

        Projector2.pivot = new Vector3(0, transform.position.y,
                                       chargeRange / 2);

        Projector1.enabled = false;
        Projector2.enabled = false;
        BoxCollider1.enabled = false;
        BoxCollider2.enabled = false;

        foreach (AnimationClip clip in Animator.runtimeAnimatorController.animationClips)
        {
            clipDict[clip.name] = clip.length;
        }
    }
}
