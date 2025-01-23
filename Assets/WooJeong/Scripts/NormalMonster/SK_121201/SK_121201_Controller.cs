using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SK_121201_Controller : NController
{
    protected override void Setup()
    {
        Animator = GetComponentInChildren<Animator>();
        foreach (AnimationClip clip in Animator.runtimeAnimatorController.animationClips)
        {
            clipDict[clip.name] = clip.length;
        }

        runState = gameObject.AddComponent<SK_121201_RunState>();
        attackState1 = gameObject.AddComponent<SK_121201_AttackState1>();
        attackState2 = gameObject.AddComponent<SK_121201_AttackState2>();
        waitState = gameObject.AddComponent<SK_121201_WaitState>();
        stunState = gameObject.AddComponent<SK_121201_StunState>();
        deathState = gameObject.AddComponent<SK_121201_DeathState>();

        player = GameObject.Find("Player");     
    }
}
