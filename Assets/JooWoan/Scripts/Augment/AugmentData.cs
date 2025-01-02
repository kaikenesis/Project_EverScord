using UnityEngine;
using System.Collections.Generic;
using EverScord.Armor;
using EverScord.FileIO;

namespace EverScord.Augment
{
    public class AugmentData
    {
        #region Expression-bodied member source
        private IDictionary<string, List<ArmorAugment>> dealerHelmetAugmentDict = new Dictionary<string, List<ArmorAugment>>();
        private IDictionary<string, List<ArmorAugment>> healerHelmetAugmentDict = new Dictionary<string, List<ArmorAugment>>();
        private IDictionary<string, List<ArmorAugment>> vestAugmentDict         = new Dictionary<string, List<ArmorAugment>>();
        private IDictionary<string, List<ArmorAugment>> shoesAugmentDict        = new Dictionary<string, List<ArmorAugment>>();
        #endregion

        public IDictionary<string, List<ArmorAugment>> DealerHelmetAugmentDict => dealerHelmetAugmentDict;
        public IDictionary<string, List<ArmorAugment>> HealerHelmetAugmentDict => healerHelmetAugmentDict;
        public IDictionary<string, List<ArmorAugment>> VestAugmentDict => vestAugmentDict;
        public IDictionary<string, List<ArmorAugment>> ShoesAugmentDict => shoesAugmentDict;

        private const string SHEET_PATH = "CSVSheet/ArmorAugmentSheet";
        private const string HELMET_KEY = "Helmet";
        private const string VEST_KEY   = "Vest";
        private const string SHOES_KEY  = "Shoes";

        public void Init()
        {
            var sheet = CSVReader.ReadAugmentSheet(SHEET_PATH);

            foreach (KeyValuePair<string, List<List<string>>> kvp in sheet)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    if (kvp.Value[i].Count == 0)
                        continue;

                    string name         = kvp.Value[i][0];
                    string description  = kvp.Value[i][1];

                    int stageCount;
                    int start           = 2;
                    int statCount       = GetStatCount(kvp.Value[i], out stageCount);

                    Debug.Log(statCount);

                    for (int j = 0; j < stageCount; j++)
                    {
                        List<(float, float)> bonusList = new();
                        int end = start + statCount * 2;

                        for (int k = start; k < end; k += 2)
                        {
                            float.TryParse(kvp.Value[i][k], out float additive);
                            float.TryParse(kvp.Value[i][k + 1], out float multiplicative);

                            bonusList.Add((additive, multiplicative));
                        }

                        start = end;

                        switch (kvp.Key)
                        {
                            case HELMET_KEY:
                            {
                                HelmetAugmentBuilder helmetAugmentBuilder = new();
                                HelmetAugment helmetAugment = helmetAugmentBuilder
                                    .SetName(name)
                                    .SetDescription(description)
                                    .SetBonus((IHelmet.BonusType)0, bonusList[0].Item1, bonusList[0].Item2)
                                    .SetBonus((IHelmet.BonusType)1, bonusList[1].Item1, bonusList[1].Item2)
                                    .SetBonus((IHelmet.BonusType)2, bonusList[2].Item1, bonusList[2].Item2)
                                    .SetBonus((IHelmet.BonusType)3, bonusList[3].Item1, bonusList[3].Item2)
                                    .SetBonus((IHelmet.BonusType)4, bonusList[4].Item1, bonusList[4].Item2)
                                    .Build();

                                if (i < 2 || i == 4)
                                {
                                    if (!dealerHelmetAugmentDict.ContainsKey(helmetAugment.Name))
                                        dealerHelmetAugmentDict[helmetAugment.Name] = new();

                                    dealerHelmetAugmentDict[helmetAugment.Name].Add(helmetAugment);
                                }

                                if (i >= 2 || i == 4)
                                {
                                    if (!healerHelmetAugmentDict.ContainsKey(helmetAugment.Name))
                                        healerHelmetAugmentDict[helmetAugment.Name] = new();

                                    healerHelmetAugmentDict[helmetAugment.Name].Add(helmetAugment);
                                }
                                break;
                            }

                            case VEST_KEY:
                            {
                                VestAugmentBuilder vestAugmentBuilder = new();
                                VestAugment vestAugment = vestAugmentBuilder
                                    .SetName(name)
                                    .SetDescription(description)
                                    .SetBonus((IVest.BonusType)0, bonusList[0].Item1, bonusList[0].Item2)
                                    .SetBonus((IVest.BonusType)1, bonusList[1].Item1, bonusList[1].Item2)
                                    .SetBonus((IVest.BonusType)2, bonusList[2].Item1, bonusList[2].Item2)
                                    .Build();

                                if (!vestAugmentDict.ContainsKey(vestAugment.Name))
                                    vestAugmentDict[vestAugment.Name] = new();

                                vestAugmentDict[vestAugment.Name].Add(vestAugment);
                                break;
                            }

                            case SHOES_KEY:
                            {
                                ShoesAugmentBuilder shoesAugmentBuilder = new();
                                ShoesAugment shoesAugment = shoesAugmentBuilder
                                    .SetName(name)
                                    .SetDescription(description)
                                    .SetBonus((IShoes.BonusType)0, bonusList[0].Item1, bonusList[0].Item2)
                                    .SetBonus((IShoes.BonusType)1, bonusList[1].Item1, bonusList[1].Item2)
                                    .SetBonus((IShoes.BonusType)2, bonusList[2].Item1, bonusList[2].Item2)
                                    .Build();

                                if (!shoesAugmentDict.ContainsKey(shoesAugment.Name))
                                    shoesAugmentDict[shoesAugment.Name] = new();

                                shoesAugmentDict[shoesAugment.Name].Add(shoesAugment);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private int GetStatCount(List<string> record, out int stageCount)
        {
            int count = 0;

            for (int i = 2; i < record.Count; i++)
            {
                if (string.IsNullOrEmpty(record[i]))
                    break;

                count++;
            }

            for (int i = count - 1; i > 1; i--)
            {
                if (count % i == 0)
                {
                    stageCount = count / i;
                    return i / 2;
                }
            }

            stageCount = 1;
            return 1;
        }
    }
}
