using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SK_112206_Controller : MonoBehaviour
{
    [SerializeField] private float distance = 7.5f;
    [SerializeField] private float moveSpeed = 3f;
    public float MoveSpeed { get { return moveSpeed; } }
    public float Distance { get { return distance; } }

    private IState currentState;

    private IState moveState;
    private IState attackState;

    void Awake()
    {
        moveState = gameObject.AddComponent<SK_112206_MoveState>();
        attackState = gameObject.AddComponent<SK_112206_AttackState>();

        MoveState();
    }
    public void Transition(IState state)
    {
        currentState = state;
        currentState.Enter();
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
