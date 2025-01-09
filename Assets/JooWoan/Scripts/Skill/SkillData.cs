using EverScord.FileIO;
using EverScord.Skill;
using System.Collections.Generic;
using UnityEngine;

public class SkillData : MonoBehaviour
{
    #region Expression-bodied member source
    private IDictionary<string, CharacterSkillInfo> skillInfoDict = new Dictionary<string, CharacterSkillInfo>();
    #endregion

    private const string SHEET_PATH = "CSVSheet/SkillSheet";
    private IDictionary<string, CharacterSkillInfo> SkillInfoDict => skillInfoDict;

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        var sheet = CSVReader.ReadDataSheet(SHEET_PATH);

        for (int i = 0; i < sheet.Count; i++)
        {
            CharacterSkillInfo info = new CharacterSkillInfo();

            info.tag = sheet[i]["index"];
            info.name = sheet[i]["name"];
            info.effectType = sheet[i]["effect_type"];
            info.triggerCondition = sheet[i]["trigger_condition"];

            info.skillTypes = CSVReader.SplitCellInt(sheet[i]["skill_type"]);
            info.skillRanges = CSVReader.SplitCellFloat(sheet[i]["skill_range"]);

            float.TryParse(sheet[i]["cooldown"], out info.cooldown);
            float.TryParse(sheet[i]["effect_value"], out info.effectValue);
            float.TryParse(sheet[i]["aoe_diameter"], out info.aoeDiameter);

            int.TryParse(sheet[i]["hit_type"], out info.hitType);
            int.TryParse(sheet[i]["weapon_type"], out int offensive);
            info.isOffensive = offensive == 1;

            skillInfoDict[info.name] = info;
        }
    }
}
