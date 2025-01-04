using EverScord.FileIO;
using EverScord.Skill;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillData : MonoBehaviour
{
    private const string SHEET_PATH = "CSVSheet/SkillSheet";

    private IDictionary<string, SkillInfo> skillDict = new Dictionary<string, SkillInfo>();

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        var sheet = CSVReader.ReadDataSheet(SHEET_PATH);

        //for (int i = 0; i < sheet.Count; i++)
        //{
        //    SkillInfo info = new SkillInfo();

        //    info.tag                = sheet[i]["index"];
        //    info.name               = sheet[i]["name"];
        //    info.effectType         = sheet[i]["effect_type"];
        //    info.triggerCondition   = sheet[i]["trigger_condition"];

        //    info.skillTypes         = SetSkillTypes(sheet[i]["skill_type"]);
        //    info.skillRanges        = SetSkillRanges(sheet[i]["skill_range"]);  

        //    float.TryParse(sheet[i]["cooldown"],        out info.cooldown);
        //    float.TryParse(sheet[i]["effect_value"],    out info.effectValue);
        //    float.TryParse(sheet[i]["aoe_diameter"],    out info.aoeDiameter);

        //    int.TryParse(sheet[i]["hit_type"],          out info.hitType);
        //    int.TryParse(sheet[i]["weapon_type"],       out int offensive);
        //    info.isOffensive = offensive == 1;

        //    skillDict[info.name] = info;
        //}
    }

    private List<int> SetSkillTypes(string cell)
    {
        List<int> skillTypes = new List<int>();
        string[] skillTypeTags = cell.Split(',');

        for (int i = 0; i < skillTypeTags.Length; i++)
        {
            int.TryParse(skillTypeTags[i], out int skillType);
            skillTypes.Add(skillType);
        }

        return skillTypes;
    }

    private List<float> SetSkillRanges(string cell)
    {
        List<float> skillRanges = new List<float>();
        string[] skillRangeTags = cell.Split(',');

        for (int i = 0; i < skillRangeTags.Length; i++)
        {
            int.TryParse(skillRangeTags[i], out int skillType);
            skillRanges.Add(skillType);
        }

        return skillRanges;
    }
}
