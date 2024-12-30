using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_122101_MoveState : MonoBehaviour, SK_122101_IState
{
    private SK_122101_Controller monsterController;
    private SK_122101_StateContext monsterStateContext;
    public GameObject player;

    private Animator animator;
    void Setup()
    {
        animator = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player");
    }

    void Start()
    {
        Setup();
    }

    public void Enter(SK_122101_Controller controller)
    {
        if (!monsterController)
            monsterController = controller;

        Debug.Log("Move");
        if (animator == null)
            Setup();
        animator.SetBool("isRun", true);
    }


    public void Update()
    {
        if (monsterController)
        {
            transform.LookAt(player.transform);

            Vector3 heading = player.transform.position - transform.position;
            float distance = heading.magnitude;
            if(distance < 0.5f)
            {
                Exit();
                return;
            }
            Vector3 moveVector = heading / distance;

            transform.Translate(monsterController.moveSpeed * Time.deltaTime * moveVector, Space.World);

            Debug.DrawRay(transform.position, moveVector * monsterController.moveSpeed, Color.red);
        }
    }

    public void Exit()
    {
        monsterController.AttackState();
        monsterController = null;
        animator.SetBool("isRun", false);
    }

    
}
