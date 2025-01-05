using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112206_AttackState : MonoBehaviour, IState
{
    private SK_112206_Controller monsterController;
    private Animator animator;
    private BoxCollider boxCollider;

    void Setup()
    {
        monsterController = GetComponent<SK_112206_Controller>();
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
            StartCoroutine(Att1());
        }
        else
        {
            StartCoroutine(Att2());
        }
    }

    IEnumerator Att1()
    {
        animator.Play("Attack1", -1, 0);
        yield return null;
        AnimatorClipInfo[] clip = animator.GetCurrentAnimatorClipInfo(0);
        float time = clip[0].clip.length;
        yield return new WaitForSeconds(time / 3);
        boxCollider.enabled = true;
        yield return new WaitForSeconds(time / 3);
        boxCollider.enabled = false;
        yield return new WaitForSeconds(time / 3);
        Exit();
    }

    IEnumerator Att2()
    {
        animator.Play("Attack2", -1, 0);
        yield return null;
        AnimatorClipInfo[] clip = animator.GetCurrentAnimatorClipInfo(0);
        float time = clip[0].clip.length / 3;
        for(int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(time / 4);
            boxCollider.enabled = true;
            yield return new WaitForSeconds(time / 4);
            boxCollider.enabled = false;
            yield return new WaitForSeconds(time / 4);
        }
        yield return new WaitForSeconds(time / 4 * 3);
        Exit();
    }

    public void Exit()
    {
        monsterController.MoveState();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("attack");
    }
}
