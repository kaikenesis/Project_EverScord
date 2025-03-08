using Photon.Pun;
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
            // 보유 재화에 변화가 생길때도 UpdateInfo호출
            PhotonConnector.OnLogin += HandleLogin;
            UIChangeName.OnChangeName += HandleChangeName;
            UIFactorSlot.OnDecreaseMoney += HandleUpdateMoney;

            Initialize();
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            PhotonConnector.OnLogin -= HandleLogin;
            UIChangeName.OnChangeName -= HandleChangeName;
            UIFactorSlot.OnDecreaseMoney -= HandleUpdateMoney;
        }

        private void Initialize()
        {
            if(PhotonNetwork.IsConnected)
                UpdateName(PhotonNetwork.NickName);
            UpdateMoney(GameManager.Instance.PlayerData.money);
        }

        #region Handle Methods
        private void HandleLogin(string name, int money)
        {
            UpdateName(name);
            UpdateMoney(money);
        }
        
        private void HandleChangeName(string name)
        {
            UpdateName(name);
        }

        private void HandleUpdateMoney(int money)
        {
            UpdateMoney(money);
        }
        #endregion // Handle Methods

        private void UpdateName(string newName)
        {
            nickName.text = $"{newName}";
        }

        private void UpdateMoney(int money)
        {
            this.money.text = $"{money}";
        }
    }
}
