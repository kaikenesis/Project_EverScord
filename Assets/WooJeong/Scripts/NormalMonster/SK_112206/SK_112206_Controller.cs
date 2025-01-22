using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SK_112206_Controller : NController
{
    protected override void Setup()
    {
        runState = gameObject.AddComponent<SK_112206_RunState>();
        attackState1 = gameObject.AddComponent<SK_112206_AttackState1>();
        attackState2 = gameObject.AddComponent<SK_112206_AttackState2>();       
        waitState = gameObject.AddComponent<SK_112206_WaitState>();
        stunState = gameObject.AddComponent<SK_112206_StunState>();
        deathState = gameObject.AddComponent<SK_112206_DeathState>();

        player = GameObject.Find("Player");
        Animator = GetComponentInChildren<Animator>();
        Projector1 = gameObject.AddComponent<DecalProjector>();
        Projector2 = gameObject.AddComponent<DecalProjector>();
        BoxCollider1 = gameObject.AddComponent<BoxCollider>();
        BoxCollider2 = gameObject.AddComponent<BoxCollider>();

        ProjectorSetup();

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
