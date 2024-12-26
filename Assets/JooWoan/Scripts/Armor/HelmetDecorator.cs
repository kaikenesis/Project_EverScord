using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Armor
{
    public class HelmetDecorator : IHelmet
    {
        private readonly IHelmet decoratedHelmet;

        public float BasicAttackDamage
        {
            get { return decoratedHelmet.BasicAttackDamage; }
        }

        public float SkillDamage
        {
            get { return decoratedHelmet.BasicAttackDamage; }
        }

        public float AllroundHealAmount
        {
            get { return decoratedHelmet.AllroundHealAmount; }
        }
    }
}
