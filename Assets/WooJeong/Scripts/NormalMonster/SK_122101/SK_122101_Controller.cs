using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SK_122101_Controller : NController
{
    protected override void Setup()
    {
        player = GameObject.Find("Player");
        animator = GetComponentInChildren<Animator>();
        runState = gameObject.AddComponent<SK_122101_RunState>();
        attackState1 = gameObject.AddComponent<SK_122101_AttackState1>();
        attackState2 = gameObject.AddComponent<SK_122101_AttackState2>();
        waitState = gameObject.AddComponent<SK_122101_WaitState>();
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
