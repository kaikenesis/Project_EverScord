using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SK_122101_Controller : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float CurrentSpeed { get; set; }
    
    private SK_122101_IState moveState;
    private SK_122101_IState attackState;

    private SK_122101_StateContext monsterStateContext;

    void Start()
    {
        monsterStateContext = new SK_122101_StateContext(this);
        moveState = gameObject.AddComponent<SK_122101_MoveState>();
        attackState = gameObject.AddComponent<SK_122101_AttackState>();

        MoveState();
    }

    public void MoveState()
    {
        monsterStateContext.Transition(moveState);
    }

    public void AttackState()
    {
        monsterStateContext.Transition(attackState);
    }
}
