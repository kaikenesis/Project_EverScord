using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SK_112301_Controller : NController
{
    [SerializeField] private float chargeRange;
    [Header("공격2 사거리")]
    [SerializeField] protected float attackRangeX2 = 0.5f;
    [SerializeField] protected float attackRangeY2 = 1f;
    [SerializeField] protected float attackRangeZ2 = 7.5f;
    public float ChargeRange { get { return chargeRange; } }
    public float AttackRangeX2 { get { return attackRangeX2; } }
    public float AttackRangeY2 { get { return attackRangeY2; } }
    public float AttackRangeZ2 { get { return attackRangeZ2; } }

    protected override void Setup()
    {
        player = GameObject.Find("Player");
        animator = GetComponentInChildren<Animator>();
        runState = gameObject.AddComponent<SK_112301_RunState>();
        attackState1 = gameObject.AddComponent<SK_112301_AttackState1>();
        attackState2 = gameObject.AddComponent<SK_112301_AttackState2>();
        waitState = gameObject.AddComponent<SK_112301_WaitState>();
        projector = gameObject.AddComponent<DecalProjector>();
        boxCollider = gameObject.AddComponent<BoxCollider>();

        ProjectorSetup();

        projector.enabled = false;
        boxCollider.enabled = false;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            clipDict[clip.name] = clip.length;
        }
    }
}
