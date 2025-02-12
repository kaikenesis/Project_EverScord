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
            REROLL_FACTOR,
            APPLY_OPTION,
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
            UIFactorPanel.OnUnlockFactor += HandleUnlockFactor;
            UIFactorPanel.OnRerollFactor += HandleRerollFactor;

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            UIFactorPanel.OnUnlockFactor -= HandleUnlockFactor;
            UIFactorPanel.OnRerollFactor -= HandleRerollFactor;
        }

        private void HandleUnlockFactor(int cost)
        {
            DisplayUnlockFactor(cost);
            
        }

        private void HandleRerollFactor(int cost)
        {
            DisplayRerollFactor(cost);
        }

        private void DisplayUnlockFactor(int cost)
        {
            // mainMsg, subMsg, acceptText, cancelText, �䱸 money, ���� money �����ͷ� �и� �ʿ�
            gameObject.SetActive(true);
            curWindowType = EType.UNLOCK_FACTOR;
            int money;
            if (GameManager.Instance.userData != null)
                money = GameManager.Instance.userData.money;
            else
                money = -1;

            if (money < cost)
            {
                SetMessage($"�ʿ��� ��ȭ : {cost}\n �����Ͻðڽ��ϱ�?", $"���� ��ȭ : <color=red>{money}</color>");
                bCanAccept = false;
            }
            else
            {
                SetMessage($"�ʿ��� ��ȭ : {cost}\n �����Ͻðڽ��ϱ�?", $"���� ��ȭ : {money}");
                bCanAccept = true;
            }

            acceptText.text = "����";
            refuseText.text = "���";
        }

        private void DisplayRerollFactor(int cost)
        {
            gameObject.SetActive(true);
            curWindowType = EType.REROLL_FACTOR;
            int money;
            if (GameManager.Instance.userData != null)
                money = GameManager.Instance.userData.money;
            else
                money = -1;

            if (money < cost)
            {
                SetMessage($"�ʿ��� ��ȭ : {cost}\n �����Ͻðڽ��ϱ�?", $"���� ��ȭ : <color=red>{money}</color>");
                bCanAccept = false;
            }
            else
            {
                SetMessage($"�ʿ��� ��ȭ : {cost}\n �����Ͻðڽ��ϱ�?", $"���� ��ȭ : {money}");
                bCanAccept = true;
            }

            acceptText.text = "����";
            refuseText.text = "���";
        }

        private void DisplayApplyOption()
        {
            gameObject.SetActive(true);
            curWindowType = EType.APPLY_OPTION;
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

            acceptText.text = "����";
            refuseText.text = "���";
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
                        if (bCanAccept == true)
                        {
                            OnAcceptUnlock?.Invoke();
                            curWindowType = EType.NONE;
                            gameObject.SetActive(false);
                        }
                    }
                    break;
                case EType.REROLL_FACTOR:
                    {
                        // OptionList���� �����ɼǰ�, ������ġ�� ����
                        // �ش� ���� �ý���â�� ǥ���Ҽ� �ֵ���
                    }
                    break;
                case EType.APPLY_OPTION:
                    {
                        // ������ �ɼ� UI Image ���� ( �ӽ÷� ���� ���� )
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
