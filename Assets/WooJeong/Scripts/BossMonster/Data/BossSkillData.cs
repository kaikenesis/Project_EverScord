using System.Collections.Generic;
using UnityEngine;
using EverScord;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/BossSkillData", order = 0)]
public class BossSkillData : ScriptableObject, IData
{
    public string Tag {  get; private set; }
    public float Cooldown { get; private set; }
    public float SkillRange { get; private set; }
    public List<float> SkillSizes { get; private set; }
    public float SkillDamage { get; private set; }
    public float MaxHpBasedDamage { get; private set; }
    public float SkillDotDamage { get; private set; }

    public void Init()
    {
        MonsterSkillInfo skill = MonsterData.MonsterInfoDict[Tag];
        Cooldown = skill.cooldown;
        SkillRange = skill.skillRange;
        SkillSizes = skill.skillSizes;
        SkillDamage = skill.skillDamage;
        MaxHpBasedDamage = skill.maxHpBasedDamage;
        SkillDotDamage = skill.skillDotDamage;
    }
}
