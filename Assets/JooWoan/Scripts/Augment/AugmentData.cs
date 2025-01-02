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
            // Initialize augment dict

            var sheet = CSVReader.ReadAugmentSheet(SHEET_PATH);

            foreach (KeyValuePair<string, List<List<string>>> kvp in sheet)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    if (kvp.Value[i].Count == 0)
                        continue;

                    string name         = kvp.Value[i][0];
                    string description  = kvp.Value[i][1];

                    for (int j = 2; j < kvp.Value[i].Count; j += 2)
                    {
                        float.TryParse(kvp.Value[i][j],     out float additive);
                        float.TryParse(kvp.Value[i][j + 1], out float multiplicative);

                        switch (kvp.Key)
                        {
                            case HELMET_KEY:
                            {
                                HelmetAugmentBuilder helmetAugmentBuilder = new();
                                HelmetAugment helmetAugment = helmetAugmentBuilder
                                    .SetName(name)
                                    .SetDescription(description)
                                    .SetBonus((IHelmet.BonusType)i, additive, multiplicative)
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
                                    .SetBonus((IVest.BonusType)i, additive, multiplicative)
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
                                    .SetBonus((IShoes.BonusType)i, additive, multiplicative)
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
    }
}
