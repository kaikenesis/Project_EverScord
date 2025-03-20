using System.Collections.Generic;
using UnityEngine;
using EverScord;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/BossSkillData", order = 1)]
public class BossSkillData : ScriptableObject, IData
{
    [SerializeField] float hardModeRate = 1.3f;
    [field : SerializeField] public string Tag {  get; private set; }
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public float SkillRange { get; private set; }
    [field: SerializeField] public List<float> SkillSizes { get; private set; }
    [field: SerializeField] public float SkillDamage { get; private set; }
    [field: SerializeField] public float MaxHpBasedDamage { get; private set; }
    [field: SerializeField] public float SkillDotDamage { get; private set; }

    public void Init()
    {
        MonsterSkillInfo skill = MonsterData.MonsterInfoDict[Tag];
        Cooldown = skill.cooldown;
        SkillRange = skill.skillRange;
        SkillSizes = skill.skillSizes;
        

        if (GameManager.Instance.PlayerData.difficulty == PlayerData.EDifficulty.Hard)
        {
            SkillDamage = skill.skillDamage * hardModeRate;
            MaxHpBasedDamage = skill.maxHpBasedDamage * hardModeRate;
            SkillDotDamage = skill.skillDotDamage * hardModeRate;
        }
        else
        {
            SkillDamage = skill.skillDamage;
            MaxHpBasedDamage = skill.maxHpBasedDamage;
            SkillDotDamage = skill.skillDotDamage;
        }
    }
}
