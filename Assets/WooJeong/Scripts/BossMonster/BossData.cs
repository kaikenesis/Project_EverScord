using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/BossData")]
public class BossData : ScriptableObject
{
    [field: SerializeField] public float Hp { get; protected set; }
    [field: SerializeField] public float MaxHp { get; protected set; }
    [field: SerializeField] public float Speed { get; protected set; }
    [field: SerializeField] public float AttackRange { get; protected set; }

    public void ReduceHp()
    {
        Hp -= 10f;
    }
}
