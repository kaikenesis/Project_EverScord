using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112206_MoveState : MonoBehaviour, IState
{
    private SK_112206_Controller monsterController;
    public GameObject player;
    private Animator animator;
    private bool isEnter = false;

    void Setup()
    {
        monsterController = GetComponent<SK_112206_Controller>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player");
    }

    void Awake()
    {
        Setup();
    }

    public void Enter()
    {
        isEnter = true;

        if (animator == null)
            Setup();

        if (CalcDistance() < monsterController.Distance)
            Exit();
        else
            animator.Play("Run", -1, 0f);
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

        if(CalcDistance() < monsterController.Distance)
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
        monsterController.AttackState();
    }


}
