using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/112301_Data")]
public class NML1_Data : NMonsterData
{
    [field: SerializeField] public float ChargeRange { get; protected set; }
}
