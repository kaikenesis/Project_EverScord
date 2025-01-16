using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SK_112206_AttackState2 : NAttackState
{
    protected override void Setup()
    {
        monsterController = GetComponent<SK_112206_Controller>();
    }

    protected override IEnumerator Attack()
    {
        yield return ProjectAttackRange();

        monsterController.Animator.CrossFade("Attack2", 0.25f);        
        float time = monsterController.clipDict["Attack2"];
        
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(time / 4 / 3);
            monsterController.BoxCollider.enabled = true;
            yield return new WaitForSeconds(time / 4 / 3);
            monsterController.BoxCollider.enabled = false;
            yield return new WaitForSeconds(time / 4 / 3);
        }
        yield return new WaitForSeconds(time / 4);
        StartCoroutine(monsterController.CoolDown2());
        Exit();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("attack");
    }
}
