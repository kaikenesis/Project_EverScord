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
            // ���� ��ȭ�� ��ȭ�� ���涧�� UpdateInfoȣ��
            PhotonConnector.OnLogin += HandleLogin;
            UIChangeName.OnChangeName += HandleChangeName;
        }

        private void OnDestroy()
        {
            PhotonConnector.OnLogin -= HandleLogin;
            UIChangeName.OnChangeName -= HandleChangeName;
        }

        #region Handle Methods
        private void HandleLogin(string name, int money)
        {
            UpdateInfo(name, money);
        }
        
        private void HandleChangeName(string name, int money)
        {
            UpdateInfo(name, money);
        }
        #endregion // Handle Methods

        private void UpdateInfo(string name, int money)
        {
            nickName.text = $"�г��� : {name}";
            this.money.text = $"��ȭ : {money}";
        }
    }
}
