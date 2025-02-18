using UnityEngine;

namespace EverScord
{
    public class MB_BTRunner : BTRunner
    {
        [SerializeField] private MiddleBossData data;
        [SerializeField] private MB_Controller owner;

        protected override void Init()
        {
            blackboard.SetValue("bDead", false);
            blackboard.SetValue("bCooldown", true);
            blackboard.SetValue("Cooldown", data.Cooldown);
            blackboard.SetValue("Phase", data.CurrentPhase());
            blackboard.SetValue("Owner", (object)owner);
        }
    }
}
