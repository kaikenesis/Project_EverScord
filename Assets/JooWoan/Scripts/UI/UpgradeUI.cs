using UnityEngine;
using TMPro;

namespace EverScord.UI
{
    public class UpgradeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI description;

        public void SetText(string text)
        {
            description.text = text;
        }
    }
}
