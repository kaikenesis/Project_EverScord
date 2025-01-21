using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/BossData")]
public class BossData : ScriptableObject
{
    public float Hp { get; protected set; }
    public int Phase { get; protected set; }
    [field: SerializeField] public float MaxHp { get; protected set; }
    [field: SerializeField] public float Speed { get; protected set; }
    [field: SerializeField] public float AttackRange { get; protected set; }

    public void ResetParams()
    {
        Hp = MaxHp;
        Phase = 1;
    }

    public void ReduceHp(int decrease)
    {
        Hp -= decrease;
        if (Hp < 0) 
            Hp = 0;
    }

    public void PhaseUp()
    {
        Phase++;
    }
}
