using EverScord.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NMM2_AttackState1 : NAttackState
{
    private CapsuleCollider capsuleCollider;
    protected NavMeshAgent navMeshAgent;

    protected override IEnumerator Attack()
    {
        yield return Run();
        monsterController.PlayAnimation("Wait");
        //yield return project = StartCoroutine(monsterController.ProjectAttackRange(1));

        monsterController.PlayAnimation("Attack1");
        float time = monsterController.ClipDict["Attack1"];

        yield return new WaitForSeconds(0.6f);
        capsuleCollider.enabled = true;
        monsterController.PlaySound("NMM2_1");
        yield return new WaitForSeconds(time - 0.6f);
        capsuleCollider.enabled = false;
        StartCoroutine(monsterController.CoolDown1());
        attack = null;
        Exit();
    }

    protected IEnumerator Run()
    {
        monsterController.PlayAnimation("Run");
        navMeshAgent.stoppingDistance = monsterController.monsterData.Skill01_RangeZ;
        while (true)
        {
            navMeshAgent.destination = monsterController.Player.transform.position;
            if (monsterController.CalcDistance() < monsterController.monsterData.Skill01_RangeZ)
            {
                yield break;
            }

            yield return null;
        }
    }

    protected override void Setup()
    {
        monsterController = GetComponent<NMM2_Controller>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.center = new Vector3(0, 0,
                                  monsterController.monsterData.Skill01_RangeZ / 2);

        capsuleCollider.radius = monsterController.monsterData.Skill01_RangeZ/2;
        capsuleCollider.isTrigger = true;
        capsuleCollider.enabled = false;
    }
}
