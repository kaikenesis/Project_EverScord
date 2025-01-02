using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_122101_AttackState : MonoBehaviour, IState
{
    private SK_122101_Controller monsterController;
    private Animator animator;
    private BoxCollider boxCollider;

    void Setup()
    {
        monsterController = GetComponent<SK_122101_Controller>();
        animator = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider>();
    }
   
    void Awake()
    {
        Setup();
    }

    public void Enter()
    {
        if(animator == null)
            Setup();

        RandomAttack();
    }

    void RandomAttack()
    {
        float rand = Random.Range(0, 100);
        if (rand < 50)
        {
            animator.SetBool("isAttack1", true);
            StartCoroutine(Att1());
        }
        else
        {
            animator.SetBool("isAttack2", true);
            StartCoroutine(Att2());
        }
    }

    IEnumerator Att1()
    {
        for(int i = 0; i < 4; i++)
        {
            float time = animator.GetCurrentAnimatorClipInfo(0).Length;
            if(i == 2)
                boxCollider.enabled = true;
            yield return new WaitForSeconds(time);
            boxCollider.enabled = false;
        }
        animator.SetBool("isAttack1", false);
        Exit();
    }

    IEnumerator Att2()
    {
        float time = animator.GetCurrentAnimatorClipInfo(0).Length;
        boxCollider.enabled = true;
        yield return new WaitForSeconds(time);
        boxCollider.enabled = false;
        animator.SetBool("isAttack2", false);
        Exit();
    }

    public void Exit()
    {
        animator.Play("Run", -1, 0f);
        monsterController.MoveState();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("attack");
    }
}
