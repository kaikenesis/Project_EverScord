using System.Collections.Generic;
using UnityEngine;
using EverScord.Armor;

namespace EverScord.Augment
{
    public class AugmentData
    {
        #region Expression-bodied member source
        private IDictionary<string, List<ArmorAugment>> dealerHelmetAugmentDict = new Dictionary<string, List<ArmorAugment>>();
        private IDictionary<string, List<ArmorAugment>> healerHelmetAugmentDict = new Dictionary<string, List<ArmorAugment>>();
        #endregion

        public IDictionary<string, List<ArmorAugment>> DealerHelmetAugmentDict => dealerHelmetAugmentDict;
        public IDictionary<string, List<ArmorAugment>> HealerHelmetAugmentDict => healerHelmetAugmentDict;

        public void Init()
        {
            // Initialize augment dict

            HelmetAugmentBuilder helmetAugmentBuilder = new();

            HelmetAugment helmetAugment = helmetAugmentBuilder
                .SetName("Basic Atk 1")
                .SetDescription("Enhances basic attack x 10")
                .SetBonus(IHelmet.BonusType.BasicAttack, 10f, 10f)
                .Build();

            HelmetAugment helmetAugment2 = helmetAugmentBuilder
                .SetName("Basic Atk 2")
                .SetDescription("Enhances basic attack x 15")
                .SetBonus(IHelmet.BonusType.BasicAttack, 15f, 15f)
                .Build();

            dealerHelmetAugmentDict[helmetAugment.Name] = new()
            {
                helmetAugment,
                helmetAugment2
            };



            helmetAugment = helmetAugmentBuilder
                .SetName("Skill Atk 1")
                .SetDescription("Enhances skill attack x 10")
                .SetBonus(IHelmet.BonusType.SkillAttack, 10f, 10f)
                .Build();

            helmetAugment2 = helmetAugmentBuilder
                .SetName("Skill Atk 2")
                .SetDescription("Enhances skill attack x 15")
                .SetBonus(IHelmet.BonusType.SkillAttack, 15, 15f)
                .Build();

            dealerHelmetAugmentDict[helmetAugment.Name] = new()
            {
                helmetAugment,
                helmetAugment2
            };



            helmetAugment = helmetAugmentBuilder
                .SetName("All round heal 1")
                .SetDescription("Enhances all round heal x 10")
                .SetBonus(IHelmet.BonusType.AllroundHeal, 10f, 10f)
                .Build();

            helmetAugment2 = helmetAugmentBuilder
                .SetName("All round heal 2")
                .SetDescription("Enhances all round heal x 15")
                .SetBonus(IHelmet.BonusType.AllroundHeal, 15, 15f)
                .Build();

            dealerHelmetAugmentDict[helmetAugment.Name] = new()
            {
                helmetAugment,
                helmetAugment2
            };
        }

        public List<ArmorAugment> GetAugment(string name)
        {
            {
                if (dealerHelmetAugmentDict.TryGetValue(name, out var augmentList))
                    return augmentList;
            }
            {
                if (healerHelmetAugmentDict.TryGetValue(name, out var augmentList))
                    return augmentList;
            }

            Debug.LogWarning($"Augment load failed : {name}");
            return null;
        }
    }
}
