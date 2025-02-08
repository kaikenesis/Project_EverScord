using System;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIPopUpWindow : MonoBehaviour
    {
        /*
        ���� ��ȭ�� �ʿ����� Textǥ��, �� ���� ��ȭ�� ������ Textǥ�� (�����ϸ� �� ����)
        ��ȭ�Ҹ� �����̸� Ȯ�� ������ ��ȭ ����

        �����ִ� ���ڿ� �����ɼ� �������� ���� �ɼ� -> ���ο� �ɼ� �̸� ������ ������ Textǥ��
        Ȯ�� ������ ���ο� �ɼ��� �����ϰ� ���� �̹��� ����
        */

        enum EType
        {
            NONE,
            UNLOCK_FACTOR,
            MAX
        }

        [SerializeField] private TMP_Text mainMessage;
        [SerializeField] private TMP_Text subMessage;
        [SerializeField] private TMP_Text acceptText;
        [SerializeField] private TMP_Text refuseText;
        [SerializeField] private PopUpWindowData data;
        private EType curWindowType = EType.NONE;
        private bool bCanAccept = false;

        public static Action OnAcceptUnlock = delegate { };
        
        private void Awake()
        {
            UIFactorSlot.OnClickedSlot += HandleClickedSlot;

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            UIFactorSlot.OnClickedSlot -= HandleClickedSlot;
        }

        private void HandleClickedSlot(bool bConfirmed, bool bLock, int type, int slotNum)
        {
            if (bConfirmed == true) return;

            if(bLock == true)
            {
                UnlockFactorWindow();
            }
            else
            {

            }
            
        }

        private void UnlockFactorWindow()
        {
            // mainMsg, subMsg, acceptText, cancelText, �䱸 money, ���� money �����ͷ� �и� �ʿ�
            gameObject.SetActive(true);
            curWindowType = EType.UNLOCK_FACTOR;
            int pay = 0;
            int money;
            if (GameManager.Instance.userData != null)
                money = GameManager.Instance.userData.money;
            else
                money = -1;

            if (money < pay)
            {
                SetMessage($"�ʿ��� ��ȭ : {pay}\n �����Ͻðڽ��ϱ�?", $"���� ��ȭ : <color=red>{money}</color>");
                bCanAccept = false;
            }
            else
            {
                SetMessage($"�ʿ��� ��ȭ : {pay}\n �����Ͻðڽ��ϱ�?", $"���� ��ȭ : {money}");
                bCanAccept = true;
            }
        }

        private void SetMessage(string mainMessage, string subMessage)
        {
            this.mainMessage.text = mainMessage;
            this.subMessage.text = subMessage;
        }

        public void OnAccepted()
        {
            switch(curWindowType)
            {
                case EType.UNLOCK_FACTOR:
                    {
                        OnAcceptUnlock?.Invoke();
                        if (bCanAccept == true)
                        {
                            curWindowType = EType.NONE;
                            gameObject.SetActive(false);
                        }
                    }
                    break;
            }
        }

        public void OnCancled()
        {
            curWindowType = EType.NONE;
            gameObject.SetActive(false);
        }
    }
}
