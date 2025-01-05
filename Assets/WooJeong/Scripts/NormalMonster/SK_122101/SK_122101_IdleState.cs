using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_122101_IdleState : MonoBehaviour, IState
{
    private SK_122101_Controller monsterController;
    private Animator animator;

    private void Setup()
    {
        monsterController = GetComponent<SK_122101_Controller>();
        animator = GetComponentInChildren<Animator>();
    }
    void Awake()
    {
        Setup();
    }

    public void Enter()
    {
        animator.Play("IDLE");
    }

    public void Exit()
    {
        monsterController.MoveState();
    }

    public void ExitToAttack()
    {
        monsterController.AttackState();
    }
}
