using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112301_RunState : NRunState
{
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

    protected override void Update()
    {
        if (!isEnter)
            return;
        
        if (monsterController.isStun)
        {
            isEnter = false;
            ExitToStun();
            return;
        }

        if (monsterController.isDead)
        {
            isEnter = false;
            ExitToDeath();
            return;
        }

        Vector3 moveVector = (monsterController.player.transform.position - transform.position).normalized;
        monsterController.LookPlayer();
        transform.Translate(monsterController.monsterData.MoveSpeed * Time.deltaTime * moveVector, Space.World);

        float distance = monsterController.CalcDistance();
        if (distance < chargeRange && monsterController.IsCoolDown(2))
        {
            isEnter = false;
            ExitToAttack2();
            return;
        }
        else if (distance < monsterController.monsterData.AttackRangeZ1 && monsterController.IsCoolDown(1))
        {
            isEnter = false;
            ExitToAttack1();
            return;
        }
        else if(distance < monsterController.monsterData.AttackRangeZ1)
        {
            isEnter = false;
            ExitToWait(); 
            return;
        }
    }
}
