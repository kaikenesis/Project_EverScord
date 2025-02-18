
namespace EverScord.Character
{
    public enum CharacterState
    {
        NONE            = 0,
        SKILL_STANCE    = (1 << 0),
        STUNNED         = (1 << 1)
    }

    public enum SetCharacterStateMode
    {
        ADD,
        REMOVE,
        CLEAR
    }
}