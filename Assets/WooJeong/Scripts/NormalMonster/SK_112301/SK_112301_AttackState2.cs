using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112301_AttackState2 : NAttackState
{
    private Vector3 moveVector;
    private Vector3 startVector;
    private float chargeRange;


    protected override void Setup()
    {
        monsterController = GetComponent<SK_112301_Controller>();
    }

    private void Start()
    {
        var temp = monsterController as SK_112301_Controller;
        chargeRange = temp.ChargeRange;
    }

    protected override IEnumerator Attack()
    {
        startVector = transform.position;
        moveVector = (monsterController.player.transform.position - transform.position).normalized;
        yield return project = StartCoroutine(monsterController.ProjectAttackRange(2));
        monsterController.PlayAnimation("Attack2");
        float time = monsterController.clipDict["Attack2"];
        
        yield return new WaitForSeconds(time / 4);
        monsterController.BoxCollider2.enabled = true;
        StartCoroutine(Charge(1f));
        yield return new WaitForSeconds(time / 4 * 3);
        monsterController.BoxCollider2.enabled = false;
        StartCoroutine(monsterController.CoolDown2());
        attack = null;
        Exit();
    }

    protected override void Update()
    {
        if (!isEnter)
            return;

        if (monsterController.isStun)
        {
            ExitToStun();
            return;
        }

        if (monsterController.isDead)
        {
            ExitToDeath();
            return;
        }

        if (canAttack)
            return;

        if (monsterController.CalcDistance() > chargeRange)
        {
            canAttack = true;
            ExitToRun();
        }

        monsterController.LookPlayer();
        if (monsterController.IsLookPlayer(chargeRange))
        {
            canAttack = true;
            attack = StartCoroutine(Attack());
        }
    }

    private IEnumerator Charge(float duration)
    {
        Vector3 endPoint = startVector + moveVector * (chargeRange - monsterController.monsterData.AttackRangeZ2);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(startVector, endPoint, t / duration);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
