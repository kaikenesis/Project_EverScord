using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UISkill : MonoBehaviour
    {
        [SerializeField] private Image skillImg;
        [SerializeField] private Image coverImg;
        [SerializeField] private Image btnImg;

        public void Initialize(Sprite skillSourceImg, Sprite btnSourceImg)
        {
            skillImg.sprite = skillSourceImg;
            btnImg.sprite = btnSourceImg;
        }

        public void ShowCooldown()
        {
            // 1 -> 0 : 1 - curCooldown / maxCooldown
            coverImg.fillAmount = 0;
        }
    }
}