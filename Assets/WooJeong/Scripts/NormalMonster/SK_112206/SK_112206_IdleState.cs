using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112206_IdleState : MonoBehaviour, IState
{
    private SK_112206_Controller monsterController;
    private int lastAttack = 0;
    private bool isEnter = false;

    void Awake()
    {
        monsterController = GetComponent<SK_112206_Controller>();
    }

    public void Enter()
    {
        isEnter = true;
        monsterController.Animator.Play("Idle");
        if (monsterController.CalcDistance() > monsterController.Distance)
        {
            Exit();
        }
        else
        {
            StartCoroutine(RandomAttack());
        }
    }
    private void Update()
    {
        if (!isEnter)
            return;

        monsterController.LookPlayer();
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
        isEnter = false;
        monsterController.MoveState();
    }

    public void ExitToAttack1()
    {
        isEnter = false;
        monsterController.AttackState1();
    }

    public void ExitToAttack2()
    {
        isEnter = false;
        monsterController.AttackState2();
    }


}
