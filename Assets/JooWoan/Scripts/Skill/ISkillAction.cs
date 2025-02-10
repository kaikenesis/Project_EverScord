using EverScord.Character;

namespace EverScord.Skill
{
    public interface ISkillAction
    {
        void Activate(EJob ejob);
        void Init(CharacterControl activator, CharacterSkill skill, int skillIndex);
        bool IsUsingSkill { get; }
    }
}
