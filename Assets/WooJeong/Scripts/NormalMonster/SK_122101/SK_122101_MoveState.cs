using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_122101_MoveState : MonoBehaviour, SK_122101_IState
{
    private SK_122101_Controller monsterController;
    public GameObject player;
    private Animator animator;
    private bool isEnter = false;
    private float distance = 0.5f;

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
        isEnter = true;
        if (!monsterController)
            monsterController = controller;

        if (animator == null)
            Setup();

        if (CalcDistance() < distance)
            Exit();
        else
            animator.SetBool("isRun", true);
    }

    float CalcDistance()
    {
        Vector3 heading = player.transform.position - transform.position;
        float distance = heading.magnitude;

        return distance;
    }

    public void Update()
    {
        if (!isEnter)
            return;

        if(CalcDistance() < distance)
        {
            Exit();
            return;
        }
        Vector3 moveVector = (player.transform.position - transform.position).normalized;
        transform.LookAt(player.transform);
        transform.Translate(monsterController.MoveSpeed * Time.deltaTime * moveVector, Space.World);

        Debug.DrawRay(transform.position, moveVector * monsterController.MoveSpeed, Color.red);
    }

    public void Exit()
    {
        isEnter = false;
        animator.SetBool("isRun", false);
        monsterController.AttackState();
    }


}
