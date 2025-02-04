using EverScord.Character;
using UnityEngine;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Grenade Skill", menuName = "EverScord/Character Skill/Grenade Skill")]
    public class GrenadeSkill : CharacterSkill
    {
        [SerializeField] private float explosionRadius;

        public override void Init(CharacterControl activator, ref SkillActionInfo skillActionInfo)
        {
            base.Init(activator, ref skillActionInfo);
            ((GrenadeSkillAction)skillActionInfo.SkillAction).Init(activator);
        }
    }
}
