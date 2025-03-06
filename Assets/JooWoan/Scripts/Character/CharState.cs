
namespace EverScord.Character
{
    public enum CharState
    {
        NONE                = 0,
        SKILL_STANCE        = (1 << 0),
        STUNNED             = (1 << 1),
        RIGID_ANIMATING     = (1 << 2),
        INVINCIBLE          = (1 << 3),
        DEATH               = (1 << 4),
        TELEPORTING         = (1 << 5),
        SELECTING_AUGMENT   = (1 << 6) 
    }

    public enum SetCharState
    {
        ADD,
        REMOVE,
        CLEAR
    }
}