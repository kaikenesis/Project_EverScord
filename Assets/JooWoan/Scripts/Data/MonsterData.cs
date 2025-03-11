using EverScord.FileIO;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class MonsterData : MonoBehaviour
    {
        private static IDictionary<string, MonsterSkillInfo> monsterInfoDict = new Dictionary<string, MonsterSkillInfo>();
        public static IDictionary<string, MonsterSkillInfo> MonsterInfoDict => monsterInfoDict;

        void Awake()
        {
            Init();
        }

        private void Init()
        {
            var sheet = CSVReader.ReadDataSheet("MonsterSheet");

            for (int i = 0; i < sheet.Count; i++)
            {
                MonsterSkillInfo info = new MonsterSkillInfo();

                info.tag = sheet[i]["tag"];
                info.name = sheet[i]["name"];
                float.TryParse(sheet[i]["cooldown"],     out info.cooldown);
                float.TryParse(sheet[i]["skill_range"],  out info.skillRange);
                info.skillSizes = CSVReader.SplitCellFloat(sheet[i]["skill_size"]);

                float.TryParse(sheet[i]["skill_dmg"],    out info.skillDamage);
                float.TryParse(sheet[i]["skill_hpbase"], out info.maxHpBasedDamage);
                float.TryParse(sheet[i]["skill_dot"],    out info.skillDotDamage);

                monsterInfoDict[info.tag] = info;
            }
        }
    }
}
