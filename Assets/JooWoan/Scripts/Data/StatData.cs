using System.Collections.Generic;
using EverScord.FileIO;

namespace EverScord
{
    public class StatData : IData
    {
        private static IDictionary<string, StatInfo> statInfoDict = new Dictionary<string, StatInfo>();
        public static IDictionary<string, StatInfo> StatInfoDict => statInfoDict;

        public void Init()
        {
            var sheet = CSVReader.ReadStatSheet("StatSheet");

            for (int i = 0; i < sheet.Count; i++)
            {
                StatInfo info = new StatInfo();

                info.tag = sheet[i]["Tag"];

                float.TryParse(sheet[i]["Health"],              out info.health);
                float.TryParse(sheet[i]["HealthRegen"],         out info.healthRegen);
                float.TryParse(sheet[i]["Attack"],              out info.attack);
                float.TryParse(sheet[i]["SupportAttack"],       out info.supportAttack);
                float.TryParse(sheet[i]["Defense"],             out info.defense);
                float.TryParse(sheet[i]["CritChance"],          out info.critChance);
                float.TryParse(sheet[i]["Speed"],               out info.speed);
                float.TryParse(sheet[i]["CooldownDecrease"],    out info.cooldownDecrease);
                float.TryParse(sheet[i]["ReloadSpeedDecrease"], out info.reloadSpeedDecrease);
                float.TryParse(sheet[i]["SkillDamageIncrease"], out info.skillDamageIncrease);
                float.TryParse(sheet[i]["HealIncrease"],        out info.healIncrease);
                
                statInfoDict[info.tag] = info;
            }
        }
    }
}
