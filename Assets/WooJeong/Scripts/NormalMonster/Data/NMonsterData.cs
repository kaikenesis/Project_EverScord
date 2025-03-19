using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EverScord;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/NormalMonsterData")]
public class NMonsterData : ScriptableObject, IData
{
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
        HP = stat.health;
        Defense = stat.defense;
        BaseAttackDamage = stat.attack;
        MoveSpeed = stat.speed * 0.01f;

        MonsterSkillInfo skill_1 = MonsterData.MonsterInfoDict[Tag + "_1"];
        CoolDown1 = skill_1.cooldown;
        Skill01_RangeZ = skill_1.skillRange;
        Skill01_RangeX = skill_1.skillSizes[0];
        Skill01_RangeY = 3;
        Skill01_Damage = skill_1.skillDamage;

        MonsterSkillInfo skill_2 = MonsterData.MonsterInfoDict[Tag + "_2"];
        CoolDown2 = skill_2.cooldown;
        Skill02_RangeZ = skill_2.skillRange;
        Skill02_RangeY = 3;
        Skill02_RangeX = skill_2.skillSizes[0];
        Skill02_Damage = skill_2.skillDamage;
    }
}
