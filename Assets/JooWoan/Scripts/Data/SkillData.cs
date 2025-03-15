using EverScord.FileIO;
using System.Collections.Generic;

namespace EverScord
{
    public class SkillData : IData
    {
        private static IDictionary<string, CharacterSkillInfo> skillInfoDict = new Dictionary<string, CharacterSkillInfo>();
        public static IDictionary<string, CharacterSkillInfo> SkillInfoDict => skillInfoDict;

        public void Init()
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
                float.TryParse(sheet[i]["skill_dot"],    out info.skillDotDamage);
                float.TryParse(sheet[i]["skill_shield"], out info.skillShield);

                float.TryParse(sheet[i]["skill_range"],  out info.skillRange);
                info.skillSizes = CSVReader.SplitCellFloat(sheet[i]["skill_size"]);

                skillInfoDict[info.tag] = info;
            }
        }
    }
}
