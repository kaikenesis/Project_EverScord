using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/NormalMonsterData")]
public class NMonsterData : ScriptableObject
{
    [field: SerializeField] public Material DecalMat { get; protected set; }
    [field: SerializeField] public float HP { get; protected set; }
    [field: SerializeField] public float MoveSpeed { get; protected set; }
    [field: SerializeField] public float CoolDown1 { get; protected set; }
    [field: SerializeField] public float CoolDown2 { get; protected set; }
    [field: SerializeField] public float ProjectionTime { get; protected set; }
    [field: SerializeField] public float AttackRangeX1 { get; protected set; }
    [field: SerializeField] public float AttackRangeY1 { get; protected set; }
    [field: SerializeField] public float AttackRangeZ1 { get; protected set; }
    [field: SerializeField] public float AttackRangeX2 { get; protected set; }
    [field: SerializeField] public float AttackRangeY2 { get; protected set; }
    [field: SerializeField] public float AttackRangeZ2 { get; protected set; }
    [field: SerializeField] public float StopDistance { get; protected set; }

    [HideInInspector] public float SmoothAngleSpeed = 20;
}
