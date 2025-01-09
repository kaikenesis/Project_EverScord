using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112206_RunState : MonoBehaviour, IState
{
    private SK_112206_Controller monsterController;
    private bool isEnter = false;

    void Awake()
    {
        monsterController = GetComponent<SK_112206_Controller>();
    }

    public void Enter()
    {
        isEnter = true;

        monsterController.Animator.CrossFade("Run", 0.25f);
    }

    public void Update()
    {
        if (!isEnter)
            return;

        if (monsterController.CalcDistance() < monsterController.Distance)
        {
            Exit();
            return;
        }

        Vector3 moveVector = (monsterController.player.transform.position - transform.position).normalized;
        //transform.LookAt(monsterController.player.transform);
        monsterController.LookPlayer();
        transform.Translate(monsterController.MoveSpeed * Time.deltaTime * moveVector, Space.World);

        Debug.DrawRay(transform.position, moveVector * monsterController.MoveSpeed, Color.red);
    }

    public void Exit()
    {
        isEnter = false;
        monsterController.IdleState();
    }


}
