using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_122101_StateContext
{
    public SK_122101_IState CurrentState
    {
        get; set;
    }

    private readonly SK_122101_Controller _monsterController;

    public SK_122101_StateContext(SK_122101_Controller monsterController)
    {
        _monsterController = monsterController;
    }

    public void Transition(SK_122101_IState state)
    {
        CurrentState = state;
        CurrentState.Enter(_monsterController);
    }
}
