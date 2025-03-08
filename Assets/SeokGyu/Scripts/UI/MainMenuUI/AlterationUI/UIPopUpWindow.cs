using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            None,
            Unlock,
            Reroll,
            Apply,
            MAX
        }

        [SerializeField] private TMP_Text mainMessage;
        [SerializeField] private TMP_Text subMessage;
        [SerializeField] private TMP_Text acceptText;
        [SerializeField] private TMP_Text refuseText;
        [SerializeField] private PopUpWindowData data;
        private EType curWindowType = EType.None;
        private bool bCanAccept = false;

        public static Action OnAcceptUnlock = delegate { };
        public static Action OnAcceptReroll = delegate { };
        public static Action OnApplyOption = delegate { };
        
        private void Awake()
        {
            UIFactorPanel.OnUnlockFactor += HandleUnlockFactor;
            UIFactorPanel.OnRerollFactor += HandleRerollFactor;
            UIFactorSlot.OnRequestApplyOption += HandleRequestApplyOption;

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            UIFactorPanel.OnUnlockFactor -= HandleUnlockFactor;
            UIFactorPanel.OnRerollFactor -= HandleRerollFactor;
            UIFactorSlot.OnRequestApplyOption -= HandleRequestApplyOption;
        }

        #region Handle Methods
        private void HandleUnlockFactor(int cost)
        {
            DisplayUnlockFactor(cost);
            
        }

        private void HandleRerollFactor(int cost)
        {
            DisplayRerollFactor(cost);
        }

        private void HandleRequestApplyOption(Color imgColor, string newName, float newValue, string curName, float curValue)
        {
            DisplayApplyOption(curName, curValue, newName, newValue);
        }
        #endregion // Handle Methods

        private void DisplayUnlockFactor(int cost)
        {
            gameObject.SetActive(true);
            curWindowType = EType.Unlock;
            int money;
            if (GameManager.Instance.PlayerData != null)
                money = GameManager.Instance.PlayerData.money;
            else
                money = -1;

            refuseText.transform.parent.GetComponent<Button>().interactable = true;
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
            curWindowType = EType.Reroll;
            int money;
            if (GameManager.Instance.PlayerData != null)
                money = GameManager.Instance.PlayerData.money;
            else
                money = -1;

            refuseText.transform.parent.GetComponent<Button>().interactable = true;
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

        private void DisplayApplyOption(string curName, float curValue, string newName, float newValue)
        {
            gameObject.SetActive(true);
            curWindowType = EType.Apply;

            refuseText.transform.parent.GetComponent<Button>().interactable = true;
            if (curName == "" || curValue == 0)
            {
                mainMessage.text = $"���� : X\n���� �� : {newName}, {newValue}\n�����Ͻðڽ��ϱ�?";
                refuseText.transform.parent.GetComponent<Button>().interactable = false;
            }
            else
            {
                mainMessage.text = $"���� : {curName}, {curValue}\n���� �� : {newName}, {newValue}\n�����Ͻðڽ��ϱ�?";
            }
            subMessage.text = "";

            acceptText.text = "����";
            refuseText.text = "����";
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
                case EType.Unlock:
                    {
                        if (bCanAccept == true)
                        {
                            OnAcceptUnlock?.Invoke();
                            curWindowType = EType.None;
                            gameObject.SetActive(false);
                        }
                    }
                    break;
                case EType.Reroll:
                    {
                        if (bCanAccept == true)
                        {
                            OnAcceptReroll?.Invoke();
                        }
                    }
                    break;
                case EType.Apply:
                    {
                        OnApplyOption?.Invoke();
                        curWindowType = EType.None;
                        gameObject.SetActive(false);
                    }
                    break;
            }
        }

        public void OnCancled()
        {
            curWindowType = EType.None;
            gameObject.SetActive(false);
        }
    }
}
