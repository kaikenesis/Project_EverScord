using EverScord.FileIO;
using EverScord.Skill;
using System.Collections.Generic;
using UnityEngine;

public class SkillData : MonoBehaviour
{
    private static IDictionary<string, CharacterSkillInfo> skillInfoDict = new Dictionary<string, CharacterSkillInfo>();
    public static IDictionary<string, CharacterSkillInfo> SkillInfoDict => skillInfoDict;

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        var sheet = CSVReader.ReadDataSheet("SkillSheet");

        for (int i = 0; i < sheet.Count; i++)
        {
            CharacterSkillInfo info = new CharacterSkillInfo();

            info.tag         = sheet[i]["tag"];
            info.name        = sheet[i]["name"];
            info.skillTypes  = CSVReader.SplitCellInt(sheet[i]["skill_type"]);
            float.TryParse(sheet[i]["cooldown"],     out info.cooldown);
            info.isOffensive = sheet[i]["effect_type"] == "1";

            float.TryParse(sheet[i]["skill_atk"],    out info.skillCoefficient);
            float.TryParse(sheet[i]["skill_dmg"],    out info.skillDamage);
            float.TryParse(sheet[i]["skill_dot"],    out info.skillDps);
            float.TryParse(sheet[i]["skill_shield"], out info.skillShield);

            float.TryParse(sheet[i]["skill_range"], out info.skillRange);
            float.TryParse(sheet[i]["skill_size"], out info.skillSize);

            skillInfoDict[info.tag] = info;

            Debug.Log($"{info.tag} : {info.name} , {info.skillTypes.Count} , {info.cooldown} , {info.isOffensive} , {info.skillCoefficient} , {info.skillDamage}, {info.skillDps}, {info.skillShield}, {info.skillRange}, {info.skillSize}");
        }
    }
}
