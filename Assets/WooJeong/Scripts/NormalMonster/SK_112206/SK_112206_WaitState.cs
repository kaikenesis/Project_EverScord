using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112206_WaitState : MonoBehaviour, IState
{
    private SK_112206_Controller monsterController;
    private bool isEnter = false;

    void Awake()
    {
        monsterController = GetComponent<SK_112206_Controller>();
    }

    public void Enter()
    {
        isEnter = true;
        monsterController.Animator.CrossFade("Wait", 0.25f);

        Exit();
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
                ExitToRun();
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
                    if(monsterController.LastAttack == 1)
                        ExitToAttack2();
                    else
                        ExitToAttack1();
                    yield break;
                }
            }
            
            yield return new WaitForSeconds(0.1f);
        }

    }

    public void Exit()
    {
        StartCoroutine(RandomAttack());
    }

    private void ExitToRun()
    {
        isEnter = false;
        monsterController.RunState();
    }
    
    private void ExitToAttack1()
    {
        isEnter = false;
        monsterController.AttackState1();
    }

    private void ExitToAttack2()
    {
        isEnter = false;
        monsterController.AttackState2();
    }


}
