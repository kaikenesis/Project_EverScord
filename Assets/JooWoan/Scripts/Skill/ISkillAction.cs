using EverScord.Character;
using UnityEngine;

namespace EverScord.Skill
{
    public interface ISkillAction
    {
        void Activate();
        void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex);
        void OffensiveAction();
        void SupportAction();
        GameObject gameObject { get; }
        bool IsUsingSkill { get; }
        bool CanAttackWhileSkill { get; }
    }
}
