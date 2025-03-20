using EverScord;
using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/BossData")]
public class BossData : ScriptableObject, IData
{
    [SerializeField] float hardModeRate = 1.3f;
    public int Phase { get; protected set; }
    [field: SerializeField] public float MaxHP { get; protected set; }
    [field: SerializeField] public float MaxHP_Phase2 { get; protected set; }

    [field: SerializeField] public float BaseAttack1 { get; protected set; }
    [field: SerializeField] public float BaseAttack2 { get; protected set; }
    [field: SerializeField] public float Defense1 { get; protected set; }
    [field: SerializeField] public float Defense2 { get; protected set; }
    [field: SerializeField] public float Speed1 { get; protected set; }
    [field: SerializeField] public float Speed2 { get; protected set; }

    [field: SerializeField] public float StopDistance { get; protected set; }

    [field: SerializeField] public BossSkillData[] SkillDatas { get; protected set; }

    public void Init()
    {
        StatInfo stat_1 = StatData.StatInfoDict["FB_A"];
        StatInfo stat_2 = StatData.StatInfoDict["FB_B"];

        if (GameManager.Instance.PlayerData.difficulty == PlayerData.EDifficulty.Hard)
        {
            MaxHP = stat_1.health * hardModeRate;
            BaseAttack1 = stat_1.attack * hardModeRate;
            Defense1 = stat_1.defense * hardModeRate;
            Speed1 = stat_1.speed * 0.01f * hardModeRate;

            MaxHP_Phase2 = stat_2.health * hardModeRate;
            BaseAttack2 = stat_2.attack * hardModeRate;
            Defense2 = stat_2.defense * hardModeRate;
            Speed2 = stat_2.speed * 0.01f * hardModeRate;
        }
        else
        {
            MaxHP = stat_1.health;
            BaseAttack1 = stat_1.attack;
            Defense1 = stat_1.defense;
            Speed1 = stat_1.speed * 0.01f;

            MaxHP_Phase2 = stat_2.health;
            BaseAttack2 = stat_2.attack;
            Defense2 = stat_2.defense;
            Speed2 = stat_2.speed * 0.01f;
        }
    }
}
