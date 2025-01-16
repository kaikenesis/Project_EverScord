using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112301_RunState : NRunState
{
    private float attackRangeZ2;
    protected override void Setup()
    {
        monsterController = GetComponent<SK_112301_Controller>();
        var temp = (SK_112301_Controller)monsterController;
        attackRangeZ2 = temp.AttackRangeZ2;

    }

    protected override void Update()
    {
        if (!isEnter)
            return;

        Vector3 moveVector = (monsterController.player.transform.position - transform.position).normalized;
        monsterController.LookPlayer();
        transform.Translate(monsterController.MoveSpeed * Time.deltaTime * moveVector, Space.World);

        float distance = monsterController.CalcDistance();
        if (distance < attackRangeZ2 && monsterController.IsCoolDown(2))
        {
            isEnter = false;
            ExitToAttack2();
            return;
        }
        else if (distance < monsterController.AttackRangeZ1 && monsterController.IsCoolDown(1))
        {
            isEnter = false;
            ExitToAttack1();
            return;
        }
        else if(distance < monsterController.AttackRangeZ1)
        {
            isEnter = false;
            ExitToWait(); 
            return;
        }
    }
}
