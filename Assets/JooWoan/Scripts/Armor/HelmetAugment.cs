using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Armor
{
    public class HelmetAugment : ArmorAugment
    {
        // Dealer
        private float basicAttackDamage;
        private float skillDamage;

        // Healer
        private float basicHealAmount;
        private float skillHealAmount;
        private float allroundHealAmount;

        public HelmetAugment()
        {
            
        }

        public float SkillDamage => skillDamage;
        public float BasicAttackDamage => basicAttackDamage;
        public float AllroundHealAmount => allroundHealAmount;
    }
}
