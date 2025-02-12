using System;
using TMPro;
using UnityEngine;

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
            // mainMsg, subMsg, acceptText, cancelText, 요구 money, 현재 money 데이터로 분리 필요
            gameObject.SetActive(true);
            curWindowType = EType.UNLOCK_FACTOR;
            int money;
            if (GameManager.Instance.userData != null)
                money = GameManager.Instance.userData.money;
            else
                money = -1;

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
            curWindowType = EType.REROLL_FACTOR;
            int money;
            if (GameManager.Instance.userData != null)
                money = GameManager.Instance.userData.money;
            else
                money = -1;

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
                SetMessage($"필요한 재화 : {pay}\n 개조하시겠습니까?", $"보유 재화 : <color=red>{money}</color>");
                bCanAccept = false;
            }
            else
            {
                SetMessage($"필요한 재화 : {pay}\n 개조하시겠습니까?", $"보유 재화 : {money}");
                bCanAccept = true;
            }

            acceptText.text = "개조";
            refuseText.text = "취소";
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
                        // OptionList에서 랜덤옵션과, 랜덤수치를 설정
                        // 해당 값을 시스템창에 표시할수 있도록
                    }
                    break;
                case EType.APPLY_OPTION:
                    {
                        // 적용한 옵션 UI Image 적용 ( 임시로 색상 변경 )
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
