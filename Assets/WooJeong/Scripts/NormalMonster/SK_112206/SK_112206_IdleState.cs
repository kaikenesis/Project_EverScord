using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112206_IdleState : MonoBehaviour, IState
{
    private SK_112206_Controller monsterController;
    private int lastAttack = 0;

    void Awake()
    {
        monsterController = GetComponent<SK_112206_Controller>();
    }

    public void Enter()
    {
        monsterController.Animator.Play("Idle");
        if (monsterController.CalcDistance() > monsterController.Distance)
        {
            Exit();
        }
        else
            StartCoroutine(RandomAttack());
    }

    IEnumerator RandomAttack()
    {
        while (true)
        {
            if (monsterController.CalcDistance() > monsterController.Distance)
            {
                Exit();
                yield break;
            }

            int transition = monsterController.CheckCoolDown();
            switch (transition)
            {
                case 1:
                {
                    ExitToAttack1();
                    yield break;
                }
                case 2:
                {
                    ExitToAttack2();
                    yield break;
                }
                case 3:
                {
                    if(lastAttack == 1)
                        ExitToAttack2();
                    else
                        ExitToAttack2();
                    yield break;
                }
            }
            
            yield return new WaitForSeconds(1f);
        }

    }

    public void Exit()
    {
        monsterController.MoveState();
    }

    public void ExitToAttack1()
    {
        monsterController.AttackState1();
    }

    public void ExitToAttack2()
    {
        monsterController.AttackState2();
    }


}
