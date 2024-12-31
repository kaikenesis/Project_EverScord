using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_122101_AttackState : MonoBehaviour, SK_122101_IState
{
    private SK_122101_Controller monsterController;

    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Enter(SK_122101_Controller controller)
    {
        if (!monsterController)
            monsterController = controller;

        if(animator == null)
            animator = GetComponentInChildren<Animator>();

        RandomAttack();
    }

    void RandomAttack()
    {
        Debug.Log(animator.GetBool("isAttack"));

        float rand = Random.Range(0, 100);
        if (rand < 100)
        {
            Debug.Log("Attack1");
            animator.SetBool("isAttack", true);
            StartCoroutine(Att1());
        }
        else
        {
            Debug.Log("Attack2");
            animator.SetBool("isAttack2", true);
            StartCoroutine(Att2());
        }
    }

    IEnumerator Att1()
    {
        for(int i = 0; i < 4; i++)
        {
            float time = animator.GetCurrentAnimatorClipInfo(0).Length;
            yield return new WaitForSeconds(time);
        }
        animator.SetBool("isAttack", false);

        animator.Play("Run", -1, 0f);

        Exit();
    }

    IEnumerator Att2()
    {
        float time = animator.GetCurrentAnimatorClipInfo(0).Length;
        yield return new WaitForSeconds(time);
        animator.SetBool("isAttack2", false);
        Exit();
    }

    public void Exit()
    {
        monsterController.MoveState();
    }

}
