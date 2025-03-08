using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIPopUpWindow : MonoBehaviour
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
                SetMessage($"필요한 재화 : {cost}\n 개방하시겠습니까?", $"보유 재화 : <color=red>{money}</color>");
                bCanAccept = false;
            }
            else
            {
                SetMessage($"필요한 재화 : {cost}\n 개방하시겠습니까?", $"보유 재화 : {money}");
                bCanAccept = true;
            }

            acceptText.text = "개방";
            refuseText.text = "취소";
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
                SetMessage($"필요한 재화 : {cost}\n 개조하시겠습니까?", $"보유 재화 : <color=red>{money}</color>");
                bCanAccept = false;
            }
            else
            {
                SetMessage($"필요한 재화 : {cost}\n 개조하시겠습니까?", $"보유 재화 : {money}");
                bCanAccept = true;
            }

            acceptText.text = "개조";
            refuseText.text = "취소";
        }

        private void DisplayApplyOption(string curName, float curValue, string newName, float newValue)
        {
            gameObject.SetActive(true);
            curWindowType = EType.Apply;

            refuseText.transform.parent.GetComponent<Button>().interactable = true;
            if (curName == "" || curValue == 0)
            {
                mainMessage.text = $"현재 : X\n적용 후 : {newName}, {newValue}\n적용하시겠습니까?";
                refuseText.transform.parent.GetComponent<Button>().interactable = false;
            }
            else
            {
                mainMessage.text = $"현재 : {curName}, {curValue}\n적용 후 : {newName}, {newValue}\n적용하시겠습니까?";
            }
            subMessage.text = "";

            acceptText.text = "적용";
            refuseText.text = "유지";
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
