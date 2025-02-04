using EverScord.Character;

namespace EverScord.Skill
{
    public interface ISkillAction
    {
        void Activate(EJob ejob);
        bool IsUsingSkill { get; }
    }
}
