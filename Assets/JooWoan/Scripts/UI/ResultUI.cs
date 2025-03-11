using TMPro;
using UnityEngine;

namespace EverScord.UI
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI killCountText;
        [SerializeField] private TextMeshProUGUI dealtDamageText;
        [SerializeField] private TextMeshProUGUI dealtHealText;
        [SerializeField] private TextMeshProUGUI usernameText;

        public void Init(int killCount, float dealtDamage, float dealtHeal, string username)
        {
            killCountText.text = $"{killCount}";
            dealtDamageText.text = $"{dealtDamage}";
            dealtHealText.text = $"{dealtHeal}";
            usernameText.text = username;
        }
    }
}
