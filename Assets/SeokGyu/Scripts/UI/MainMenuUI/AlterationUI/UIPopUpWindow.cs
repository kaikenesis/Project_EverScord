using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIPopUpWindow : ToggleObject
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

        [SerializeField] private TMP_Text requireText;
        [SerializeField] private TMP_Text beforeOption;
        [SerializeField] private TMP_Text afterOption;
        [SerializeField] private TMP_Text mainMessage;
        [SerializeField] private TMP_Text subMessage;
        [SerializeField] private TMP_Text acceptText;
        [SerializeField] private TMP_Text cancelText;
        [SerializeField] private PopUpWindowData data;
        private RectTransform rectTransform;
        private EType curType = EType.None;
        private bool bCanAccept = false;

        public static Action OnAcceptUnlock = delegate { };
        public static Action OnAcceptReroll = delegate { };
        public static Action OnApplyOption = delegate { };
        
        private void Awake()
        {
            UIFactorPanel.OnUnlockFactor += HandleUnlockFactor;
            UIFactorPanel.OnRerollFactor += HandleRerollFactor;
            UIFactorSlot.OnRequestApplyOption += HandleRequestApplyOption;

            rectTransform = GetComponent<RectTransform>();

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
            OnActivateObject(0);
            OnActivateObject(1);
            OnDeactivateObject(2);
            subMessage.gameObject.SetActive(false);
            curType = EType.Unlock;
            int money = GameManager.Instance.PlayerData.money;
            rectTransform.sizeDelta = new Vector2(890f, 560f);

            cancelText.transform.parent.GetComponent<Button>().interactable = true;
            PopUpWindowData.Message msg = data.Messages[(int)curType - 1];
            SetMessage(msg.MainMessage, msg.SubMessage, msg.AcceptText, msg.CancelText);

            if (money < cost)
            {
                requireText.text = $"<color=red>{money}</color> / {cost}";
                bCanAccept = false;
            }
            else
            {
                requireText.text = $"{money} / {cost}";
                bCanAccept = true;
            }
        }

        private void DisplayRerollFactor(int cost)
        {
            OnActivateObject(0);
            OnActivateObject(1);
            OnDeactivateObject(2);
            subMessage.gameObject.SetActive(false);
            curType = EType.Reroll;
            int money = GameManager.Instance.PlayerData.money;
            rectTransform.sizeDelta = new Vector2(890f, 560f);

            cancelText.transform.parent.GetComponent<Button>().interactable = true;
            PopUpWindowData.Message msg = data.Messages[(int)curType - 1];
            SetMessage(msg.MainMessage, msg.SubMessage, msg.AcceptText, msg.CancelText);

            if (money < cost)
            {
                requireText.text = $"<color=red>{money}</color> / {cost}";
                bCanAccept = false;
            }
            else
            {
                requireText.text = $"{money} / {cost}";
                bCanAccept = true;
            }
        }

        private void DisplayApplyOption(string curName, float curValue, string newName, float newValue)
        {
            OnActivateObject(0);
            OnDeactivateObject(1);
            OnActivateObject(2);
            subMessage.gameObject.SetActive(true);
            curType = EType.Apply;
            rectTransform.sizeDelta = new Vector2(1040f, 680f);

            cancelText.transform.parent.GetComponent<Button>().interactable = true;
            PopUpWindowData.Message msg = data.Messages[(int)curType - 1];
            SetMessage(msg.MainMessage, msg.SubMessage, msg.AcceptText, msg.CancelText);
            
            afterOption.text = $"{newName}\n{newValue}%";

            if (curName == "" || curValue == 0)
            {
                beforeOption.text = "����";
                cancelText.transform.parent.GetComponent<Button>().interactable = false;
            }
            else
            {
                beforeOption.text = $"{curName}\n{curValue}%";
            }
        }

        private void SetMessage(string mainMsg, string subMsg, string acceptMsg, string cancelMsg)
        {
            mainMessage.text = mainMsg;
            subMessage.text = subMsg;
            acceptText.text = acceptMsg;
            cancelText.text = cancelMsg;
        }

        public void OnAccepted()
        {
            switch(curType)
            {
                case EType.Unlock:
                    {
                        if (bCanAccept == true)
                        {
                            OnAcceptUnlock?.Invoke();
                            curType = EType.None;
                            OnDeactivateObject(0);
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
                        curType = EType.None;
                        OnDeactivateObject(0);
                    }
                    break;
            }
        }

        public void OnCancled()
        {
            curType = EType.None;
            OnDeactivateObject(0);
        }
    }
}
