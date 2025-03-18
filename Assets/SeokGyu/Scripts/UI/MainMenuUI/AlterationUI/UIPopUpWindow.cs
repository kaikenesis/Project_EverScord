using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIPopUpWindow : ToggleObject
    {
        /*
        얼마의 재화가 필요한지 Text표시, 내 보유 재화가 얼마인지 Text표시 (부족하면 색 변경)
        재화소모 관련이면 확인 누르면 재화 연산

        열려있는 인자에 랜덤옵션 돌릴때는 기존 옵션 -> 새로운 옵션 이를 적용할 것인지 Text표시
        확인 누를시 새로운 옵션을 적용하고 인자 이미지 변경
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
                beforeOption.text = "없음";
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
