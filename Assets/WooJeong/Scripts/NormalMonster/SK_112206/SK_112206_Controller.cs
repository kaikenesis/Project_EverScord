using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class SK_112206_Controller : MonoBehaviour
{
    [SerializeField] private float distance = 7.5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int coolDown1;
    [SerializeField] private int coolDown2;
    public float MoveSpeed { get { return moveSpeed; } }
    public float Distance { get { return distance; } }

    private IState currentState;

    private IState moveState;
    private IState attackState;
    private IState idleState;

    void Awake()
    {
        moveState = gameObject.AddComponent<SK_112206_MoveState>();
        attackState = gameObject.AddComponent<SK_112206_AttackState>();
        idleState = gameObject.AddComponent<SK_112206_IdleState>();

        IdleState();
    }
    public void Transition(IState state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void IdleState()
    {
        Transition(idleState);
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
