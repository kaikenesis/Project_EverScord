using System.Collections.Generic;
using UnityEngine;
using EverScord.Armor;

namespace EverScord.Augment
{
    public class AugmentData
    {
        #region Public expression-bodied members
        public IDictionary<string, List<HelmetAugment>> DealerHelmetAugmentDict => dealerHelmetAugmentDict;
        public IDictionary<string, List<HelmetAugment>> HealerHelmetAugmentDict => healerHelmetAugmentDict;
        #endregion

        private IDictionary<string, List<HelmetAugment>> dealerHelmetAugmentDict = new Dictionary<string, List<HelmetAugment>>();
        private IDictionary<string, List<HelmetAugment>> healerHelmetAugmentDict = new Dictionary<string, List<HelmetAugment>>();


        public void Init()
        {
            // Initialize augment dict

            HelmetAugmentBuilder helmetAugmentBuilder = new();

            HelmetAugment helmetAugment = helmetAugmentBuilder
                .SetName("B Atk 1")
                .SetDescription("Enhances basic attack x 10")
                .SetBonus(IHelmet.BonusType.BasicAttack, 10f, 10f)
                .Build();

            HelmetAugment helmetAugment2 = helmetAugmentBuilder
                .SetName("B Atk 2")
                .SetDescription("Enhances basic attack x 15")
                .SetBonus(IHelmet.BonusType.BasicAttack, 15f, 15f)
                .Build();

            dealerHelmetAugmentDict[helmetAugment.Name].Add(helmetAugment);
            dealerHelmetAugmentDict[helmetAugment.Name].Add(helmetAugment2);

        }

        /*
        public ArmorAugment GetAugment(string name)
        {
            {
                if (dealerHelmetAugmentDict.TryGetValue(name, out HelmetAugment augment))
                    return augment;
            }
            {
                if (healerHelmetAugmentDict.TryGetValue(name, out HelmetAugment augment))
                    return augment;
            }

            Debug.LogWarning($"Augment load failed : {name}");
            return null;
        }
        */
    }
}
