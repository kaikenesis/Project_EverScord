using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112206_IdleState : MonoBehaviour, IState
{
    private SK_112206_Controller monsterController;
    private Animator animator;
    public GameObject player;

    private void Setup()
    {
        monsterController = GetComponent<SK_112206_Controller>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player");
    }
    void Awake()
    {
        Setup();
    }

    float CalcDistance()
    {
        Vector3 heading = player.transform.position - transform.position;
        float distance = heading.magnitude;

        return distance;
    }

    public void Enter()
    {
        animator.Play("Idle");
        if (CalcDistance() > monsterController.Distance)
        {
            Exit();
        }
        else
        {
            ExitToAttack();
        }
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
