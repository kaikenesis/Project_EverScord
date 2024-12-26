using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Armor
{
    public interface IHelmet
    {
        float BasicAttackDamage { get; }
        float SkillDamage { get; }
        float AllroundHealAmount { get; }
    }
}
