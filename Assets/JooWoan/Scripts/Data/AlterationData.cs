using System.Collections.Generic;
using UnityEngine;
using EverScord.FileIO;

namespace EverScord
{
    public class AlterationParsedData : IData
    {
        private static IDictionary<string, AlterationInfo> alterationDict = new Dictionary<string, AlterationInfo>();
        public static IDictionary<string, AlterationInfo> AlterationDict => alterationDict;

        public void Init()
        {
            var sheet = CSVReader.ReadDataSheet("AlterationSheet");

            for (int i = 0; i < sheet.Count; i++)
            {
                AlterationInfo info = new AlterationInfo();

                info.tag         = sheet[i]["tag"];
                info.abilityName = sheet[i]["ability_enhance"];
                float.TryParse(sheet[i]["ability_min"], out info.minAbility);
                float.TryParse(sheet[i]["ability_max"], out info.maxAbility);

                alterationDict[info.tag] = info;
            }
        }
    }
}
