using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EverScord;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/NormalMonsterData")]
public class NMonsterData : ScriptableObject, IData
{
    [SerializeField] float hardModeRate = 1.3f;
    [field: SerializeField] public string Tag { get; protected set; }
    [field: SerializeField] public float HP { get; protected set; }
    [field: SerializeField] public float Defense { get; protected set; }
    [field: SerializeField] public float MoveSpeed { get; protected set; }
    [field: SerializeField] public float StopDistance { get; protected set; }
    [field: SerializeField] public float CoolDown1 { get; protected set; }
    [field: SerializeField] public float CoolDown2 { get; protected set; }
    [field: SerializeField] public float ProjectionTime { get; protected set; }
    [field: SerializeField] public float BaseAttackDamage { get; protected set; }

    [field: SerializeField] public float Skill01_Damage { get; protected set; }
    [field: SerializeField] public float Skill01_MaxHpBasedDamage { get; protected set; }
    [field: SerializeField] public float Skill01_RangeX { get; protected set; }
    [field: SerializeField] public float Skill01_RangeY { get; protected set; }
    [field: SerializeField] public float Skill01_RangeZ { get; protected set; }

    [field: SerializeField] public float Skill02_Damage { get; protected set; }
    [field: SerializeField] public float Skill02_MaxHpBasedDamage { get; protected set; }
    [field: SerializeField] public float Skill02_RangeX { get; protected set; }
    [field: SerializeField] public float Skill02_RangeY { get; protected set; }
    [field: SerializeField] public float Skill02_RangeZ { get; protected set; }

    [HideInInspector] public float SmoothAngleSpeed = 20;

    public void Init()
    {
        StatInfo stat = StatData.StatInfoDict[Tag];
        MonsterSkillInfo skill_1 = MonsterData.MonsterInfoDict[Tag + "_1"];
        MonsterSkillInfo skill_2 = MonsterData.MonsterInfoDict[Tag + "_2"];
        
        CoolDown1 = skill_1.cooldown;
        Skill01_RangeZ = skill_1.skillRange;
        Skill01_RangeX = skill_1.skillSizes[0];
        Skill01_RangeY = 3;

        CoolDown2 = skill_2.cooldown;
        Skill02_RangeZ = skill_2.skillRange;
        Skill02_RangeY = 3;
        Skill02_RangeX = skill_2.skillSizes[0];

        if (GameManager.Instance.PlayerData.difficulty == PlayerData.EDifficulty.Hard)
        {
            HP = stat.health * hardModeRate;
            Defense = stat.defense * hardModeRate;
            BaseAttackDamage = stat.attack * hardModeRate;
            MoveSpeed = stat.speed * 0.01f * hardModeRate;
            
            Skill01_MaxHpBasedDamage = skill_1.maxHpBasedDamage * hardModeRate;
            Skill02_MaxHpBasedDamage = skill_2.maxHpBasedDamage * hardModeRate;
            
            Skill01_Damage = skill_1.skillDamage * hardModeRate;
            Skill02_Damage = skill_2.skillDamage * hardModeRate;
        }
        else
        {
            HP = stat.health;
            Defense = stat.defense;
            BaseAttackDamage = stat.attack;
            MoveSpeed = stat.speed * 0.01f;

            Skill01_MaxHpBasedDamage = skill_1.maxHpBasedDamage;
            Skill02_MaxHpBasedDamage = skill_2.maxHpBasedDamage;

            Skill01_Damage = skill_1.skillDamage;
            Skill02_Damage = skill_2.skillDamage;
        }
    }
}
