using System;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIAlteration : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyText;

        private void Awake()
        {
            PlayerData.OnUpdateMoney += HandleUpdateMoney;
        }

        private void OnDestroy()
        {
            PlayerData.OnUpdateMoney -= HandleUpdateMoney;
        }

        private void OnEnable()
        {
            HandleUpdateMoney(GameManager.Instance.userData.money);
        }

        private void HandleUpdateMoney(int money)
        {
            moneyText.text = $"¿Á»≠ : {money}";
        }
    }
}
