using System;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIPartyOption : MonoBehaviour
    {
        // ��Ƽ�� : ��Ƽ ������
        // ��Ƽ�� : ��Ƽ ������, ��Ƽ�� �߹�
        [SerializeField] private GameObject exileButton;
        private string nickName;

        public static Action OnClickedExit = delegate { };
        public static Action<string> OnClickedExile = delegate { };

        private void Awake()
        {
            UIRoomPlayer.OnDisplayPartyOption += HandleDisplayPartyOption;

            nickName = "";
        }

        private void OnDestroy()
        {
            UIRoomPlayer.OnDisplayPartyOption -= HandleDisplayPartyOption;
        }

        private void HandleDisplayPartyOption(bool isMasterClient, string targetName, string myName)
        {
            if (isMasterClient)
            {
                nickName = targetName;
                gameObject.SetActive(true);
                exileButton.SetActive(true);
            }
            else if(targetName == myName)
            {
                gameObject.SetActive(true);
                exileButton.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void ClickedExit()
        {
            OnClickedExit?.Invoke();
            gameObject.SetActive(false);
        }

        public void ClickedExile()
        {
            OnClickedExile?.Invoke(nickName);
            gameObject.SetActive(false);
        }
    }
}