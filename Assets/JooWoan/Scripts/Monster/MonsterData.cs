using EverScord.FileIO;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Monster
{
    public class MonsterData : MonoBehaviour
    {
        #region Expression-bodied member source
        private IDictionary<string, MonsterSkillInfo> monsterInfoDict = new Dictionary<string, MonsterSkillInfo>();
        #endregion

        private const string SHEET_PATH = "CSVSheet/MonsterSheet";
        public IDictionary<string, MonsterSkillInfo> MonsterInfoDict => monsterInfoDict;

        void Awake()
        {
            Init();
        }

        private void Init()
        {
            var sheet = CSVReader.ReadDataSheet(SHEET_PATH);

            for (int i = 0; i < sheet.Count; i++)
            {
                MonsterSkillInfo info = new MonsterSkillInfo();

                info.tag = sheet[i]["index"];
                info.name = sheet[i]["name"];

                info.monsterStages = CSVReader.SplitCellInt(sheet[i]["skill_type"]);
                info.skillRanges = CSVReader.SplitCellFloat(sheet[i]["skill_range"]);

                float.TryParse(sheet[i]["cooldown"], out info.cooldown);
                float.TryParse(sheet[i]["aoe_diameter"], out info.aoeDiameter);

                int.TryParse(sheet[i]["monster_type"], out info.monsterType);
                int.TryParse(sheet[i]["skill_type"], out info.skillType);

                monsterInfoDict[info.tag] = info;

                Debug.Log($"{info.tag} : {info.name} , {info.cooldown} , {info.aoeDiameter} , {info.monsterType} , {info.skillType}");
            }
        }
    }
}
