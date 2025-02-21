using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIAlteration : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyText;

        private void Awake()
        {
            UIFactorSlot.OnDecreaseMoney += HandleUpdateMoney;
        }

        private void OnDestroy()
        {
            UIFactorSlot.OnDecreaseMoney -= HandleUpdateMoney;
        }

        private void OnEnable()
        {
            HandleUpdateMoney(GameManager.Instance.PlayerData.money);
        }

        private void HandleUpdateMoney(int money)
        {
            moneyText.text = $"¿Á»≠ : {money}";
        }
    }
}
