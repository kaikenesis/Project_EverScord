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
        private IDictionary<string, List<ArmorAugment>> vestAugmentDict         = new Dictionary<string, List<ArmorAugment>>();
        private IDictionary<string, List<ArmorAugment>> shoesAugmentDict        = new Dictionary<string, List<ArmorAugment>>();
        #endregion

        public IDictionary<string, List<ArmorAugment>> DealerHelmetAugmentDict => dealerHelmetAugmentDict;
        public IDictionary<string, List<ArmorAugment>> HealerHelmetAugmentDict => healerHelmetAugmentDict;
        public IDictionary<string, List<ArmorAugment>> VestAugmentDict => vestAugmentDict;
        public IDictionary<string, List<ArmorAugment>> ShoesAugmentDict => shoesAugmentDict;

        public void Init()
        {
            // Initialize augment dict

            HelmetAugmentBuilder helmetAugmentBuilder = new();

            {
                HelmetAugment helmetAugment = helmetAugmentBuilder
                    .SetName("Basic Atk 1")
                    .SetDescription("Basic attack x 10")
                    .SetBonus(IHelmet.BonusType.BasicAttack, 10f, 10f)
                    .Build();

                helmetAugmentBuilder.ResetBonus();
                HelmetAugment helmetAugment2 = helmetAugmentBuilder
                    .SetName("Basic Atk 2")
                    .SetDescription("Basic attack x 15")
                    .SetBonus(IHelmet.BonusType.BasicAttack, 15f, 15f)
                    .Build();

                dealerHelmetAugmentDict[helmetAugment.Name] = new()
                {
                    helmetAugment,
                    helmetAugment2
                };
            }

            {
                helmetAugmentBuilder.ResetBonus();
                HelmetAugment helmetAugment = helmetAugmentBuilder
                    .SetName("Skill Atk 1")
                    .SetDescription("Skill attack x 10")
                    .SetBonus(IHelmet.BonusType.SkillAttack, 10f, 10f)
                    .Build();

                helmetAugmentBuilder.ResetBonus();
                HelmetAugment helmetAugment2 = helmetAugmentBuilder
                    .SetName("Skill Atk 2")
                    .SetDescription("Skill attack x 15")
                    .SetBonus(IHelmet.BonusType.SkillAttack, 15, 15f)
                    .Build();

                dealerHelmetAugmentDict[helmetAugment.Name] = new()
                {
                    helmetAugment,
                    helmetAugment2
                };
            }

            {
                helmetAugmentBuilder.ResetBonus();
                HelmetAugment helmetAugment = helmetAugmentBuilder
                        .SetName("All round heal 1")
                        .SetDescription("All-round heal x 10")
                        .SetBonus(IHelmet.BonusType.AllroundHeal, 10f, 10f)
                        .Build();

                helmetAugmentBuilder.ResetBonus();
                HelmetAugment helmetAugment2 = helmetAugmentBuilder
                    .SetName("All round heal 2")
                    .SetDescription("All-round heal x 15")
                    .SetBonus(IHelmet.BonusType.AllroundHeal, 15, 15f)
                    .Build();

                dealerHelmetAugmentDict[helmetAugment.Name] = new()
                {
                    helmetAugment,
                    helmetAugment2
                };
            }
        }
    }
}
