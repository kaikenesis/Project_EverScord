
namespace EverScord.Character
{
    public enum CharState
    {
        NONE            = 0,
        SKILL_STANCE    = (1 << 0),
        STUNNED         = (1 << 1),
        RIGID_ANIMATING = (1 << 2),
        INVINCIBLE      = (1 << 3),
        DEAD            = (1 << 4)
    }

    public enum SetCharState
    {
        ADD,
        REMOVE,
        CLEAR
    }
}