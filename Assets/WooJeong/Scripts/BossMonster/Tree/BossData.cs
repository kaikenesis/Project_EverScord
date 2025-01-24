using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/BossData")]
public class BossData : ScriptableObject
{
    public bool CanAttack {  get; protected set; }
    public float HP { get; protected set; }
    public int Phase { get; protected set; }
    [field: SerializeField] public float MaxHP { get; protected set; }
    [field: SerializeField] public float Speed { get; protected set; }
    [field: SerializeField] public float AttackRange { get; protected set; }

    public void ResetParams()
    {       
        HP = MaxHP;
        Phase = 1;
        CanAttack = false;
    }

    public void SetIsCoolDown(bool b)
    {
        CanAttack = b;
    }

    public void ReduceHp(int decrease)
    {
        HP -= decrease;
        if (HP < 0) 
            HP = 0;
    }

    public void PhaseUp()
    {
        Phase++;
        HP = 50;
    }
}
