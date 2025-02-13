using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Skill
{
    public class BombDeliveryAction : MonoBehaviour, ISkillAction
    {
        public bool IsUsingSkill => throw new System.NotImplementedException();
        public bool CanAttackWhileSkill => false;

        public void Activate()
        {
            throw new System.NotImplementedException();
        }

        public void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
        {
            throw new System.NotImplementedException();
        }
    }
}
