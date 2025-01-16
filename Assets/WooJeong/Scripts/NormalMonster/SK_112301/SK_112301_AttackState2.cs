using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112301_AttackState2 : NAttackState
{
    private Vector3 moveVector;
    private float attackRangeX2;
    private float attackRangeY2;
    private float attackRangeZ2;
    private float chargeRange;

    protected override void Setup()
    {
        monsterController = GetComponent<SK_112301_Controller>();
        var temp = (SK_112301_Controller)monsterController;
        attackRangeX2 = temp.AttackRangeX2;
        attackRangeY2 = temp.AttackRangeY2;
        attackRangeZ2 = temp.AttackRangeZ2;
        chargeRange = temp.ChargeRange;
    }

    protected override IEnumerator Attack()
    {
        yield return ProjectAttackRange();

        moveVector = (monsterController.player.transform.position - transform.position).normalized;
        monsterController.Animator.CrossFade("Attack2", 0.25f);
        float time = monsterController.clipDict["Attack2"];

        yield return new WaitForSeconds(time / 4);
        monsterController.BoxCollider.enabled = true;
        StartCoroutine(Charge(time / 4 * 3));
        yield return new WaitForSeconds(time / 4 * 3);
        monsterController.BoxCollider.enabled = false;
        StartCoroutine(monsterController.CoolDown2());
        Exit();
    }

    protected override IEnumerator ProjectAttackRange()
    {
        monsterController.Projector.size = new Vector3(attackRangeX2, attackRangeY2, attackRangeZ2);
        monsterController.Projector.pivot = new Vector3(0, 0, attackRangeZ2 / 2);
        monsterController.BoxCollider.center = new Vector3(0, 0, attackRangeZ2 / 2);
        monsterController.BoxCollider.size = new Vector3(attackRangeX2, attackRangeY2, attackRangeZ2);
        monsterController.Projector.enabled = true;
        yield return new WaitForSeconds(monsterController.ProjectionTime);
        monsterController.Projector.enabled = false;
    }

    private IEnumerator Charge(float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(transform.position, moveVector * chargeRange, t / duration);
            yield return null;
        }
    }
}
