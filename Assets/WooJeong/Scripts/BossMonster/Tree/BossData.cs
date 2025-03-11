using EverScord;
using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/BossData")]
public class BossData : ScriptableObject
{
    [field: SerializeField] public float HP { get; protected set; }
    [field: SerializeField] public int Phase { get; protected set; }
    [field: SerializeField] public float MaxHP { get; protected set; }
    [field: SerializeField] public float MaxHP_Phase2 { get; protected set; }
    [field: SerializeField] public float Speed { get; protected set; }
    [field: SerializeField] public float StopDistance { get; protected set; }
    [field: SerializeField] public float AttackRange { get; protected set; }

    public bool IsUnderHP(float hp)
    {
        if (HP > MaxHP/100 * hp)
        {
            return false;
        }
        return true;
    }
}
