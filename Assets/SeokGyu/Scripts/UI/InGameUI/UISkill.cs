using EverScord.Skill;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UISkill : MonoBehaviour
    {
        [SerializeField] private Image coverImg;

        public void Initialize()
        {
        }

        public void ShowCooldown()
        {
            // 1 -> 0 : 1 - curCooldown / maxCooldown
            coverImg.fillAmount = 0;
        }
    }
}