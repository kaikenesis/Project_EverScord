using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SK_122101_Controller : MonoBehaviour
{
    private float moveSpeed = 3f;
    public float MoveSpeed { get { return moveSpeed; } }

    private SK_122101_IState currentState;

    private SK_122101_IState moveState;
    private SK_122101_IState attackState;

    void Start()
    {
        moveState = gameObject.AddComponent<SK_122101_MoveState>();
        attackState = gameObject.AddComponent<SK_122101_AttackState>();

        MoveState();
    }
    public void Transition(SK_122101_IState state)
    {
        currentState = state;
        currentState.Enter(this);
    }

    public void MoveState()
    {
        Transition(moveState);
    }

    public void AttackState()
    {
        Transition(attackState);
    }
}
