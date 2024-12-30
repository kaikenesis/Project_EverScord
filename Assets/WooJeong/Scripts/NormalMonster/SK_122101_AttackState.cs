using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_122101_AttackState : MonoBehaviour, SK_122101_IState
{
    private SK_122101_Controller monsterController;
    private SK_122101_StateContext monsterStateContext;

    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void Enter(SK_122101_Controller controller)
    {
        if (!monsterController)
            monsterController = controller;
        animator.SetBool("isAttack", true);
        Debug.Log("Attack");

        StartCoroutine("Wait");
    }

    public void Exit()
    {
        monsterController.MoveState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);
        animator.SetBool("isAttack", false);
        Exit();
    }
}
