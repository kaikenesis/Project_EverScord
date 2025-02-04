using System;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIPlayerInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text nickName;
        [SerializeField] private TMP_Text money;

        private void Awake()
        {
            PhotonConnector.OnLogin += HandleLogin;
        }

        private void OnDestroy()
        {
            PhotonConnector.OnLogin -= HandleLogin;
        }

        private void HandleLogin(string name, int money)
        {
            UpdateInfo(name, money);
        }

        private void UpdateInfo(string name, int money)
        {
            nickName.text = $"NickName : {name}";
            this.money.text = $"Money : {money}";
        }
    }
}
