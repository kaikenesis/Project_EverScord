using UnityEngine;
using TMPro;
using EverScord.Augment;

namespace EverScord.UI
{
    public class UpgradeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI description;

        public void SetText(ArmorAugment augment, float previousIncrease, float increase)
        {
            previousIncrease = Mathf.Ceil(previousIncrease * 10) * 0.1f;
            increase         = Mathf.Ceil(increase * 10) * 0.1f;

            if (previousIncrease < 0)
                previousIncrease *= -1;
            
            if (increase < 0)
                increase *= -1;

            string previousText = augment.Description.Replace("@", $"{previousIncrease:G}%");
            string upgradeText = augment.Description.Replace("@", $"{increase:G}%");

            description.text = $"{previousText}\n{upgradeText}";
        }

        public void SetText(string text)
        {
            description.text = text;
        }
    }
}