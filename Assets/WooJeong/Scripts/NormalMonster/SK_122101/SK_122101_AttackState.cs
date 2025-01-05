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
        if (rand > 50)
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
        animator.Play("AttackA_Start", -1, 0);
        yield return null;
        string lastClip = null;
        for (int i = 0; i < 4;)
        {
            AnimatorClipInfo[] clip = animator.GetCurrentAnimatorClipInfo(0);
            if (lastClip == clip[0].clip.name)
            {
                yield return null;
                continue;
            }    
            float time = clip[0].clip.length;
            if (i == 2)
                boxCollider.enabled = true;
            yield return new WaitForSeconds(time);
            boxCollider.enabled = false;
            lastClip = clip[0].clip.name;
            i++;
        }
        Exit();
    }

    IEnumerator Att2()
    {
        animator.Play("AttackC", -1, 0);
        yield return null;
        AnimatorClipInfo[] clip = animator.GetCurrentAnimatorClipInfo(0);
        float time = clip[0].clip.length;
        boxCollider.enabled = true;
        yield return new WaitForSeconds(time);
        boxCollider.enabled = false;
        Exit();
    }

    public void Exit()
    {
        monsterController.IdleState();
    }

    private void OnTriggerEnter(Collider other)
    {
    }
}
