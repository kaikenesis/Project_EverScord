using System;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIFactorSlot : MonoBehaviour
    {
        public enum EType
        {
            ALPHA,
            BETA,
            MAX
        }

        [SerializeField] private Image backImg;
        [SerializeField] private Image lockImg;
        [SerializeField] private Image optionImg;
        private EType curType;
        private int slotNum;

        public bool bLock { get; private set; }
        public bool bConfirmed { get; private set; }
        public string curOptionName { get; private set; }
        public float curOptionValue { get; private set; }

        public static Action<int, int> OnClickedSlot = delegate { };
        public static Action<int> OnDisplayOptionList = delegate { };

        private void Awake()
        {
            UIFactor.OnRequestUnlock += HandleRequestUnlock;
            UIFactor.OnApplyOption += HandleApplyOption;

            bLock = true;
            optionImg.enabled = false;
        }

        private void OnDestroy()
        {
            UIFactor.OnRequestUnlock -= HandleRequestUnlock;
            UIFactor.OnApplyOption -= HandleApplyOption;
        }

        #region Handle Methods
        private void HandleRequestUnlock(int type, int slotNum)
        {
            if (bLock == true && curType == (EType)type && this.slotNum == slotNum)
            {
                int cost = GameManager.Instance.CostDatas.SlotCostDatas[slotNum - 1].Unlock;
                GameManager.Instance.userData.DecreaseMoney(cost);
                bLock = false;
                lockImg.enabled = false;
            }
        }

        private void HandleApplyOption(int type, int slotNum, Color optionImgColor)
        {
            if (bLock == false && curType == (EType)type && this.slotNum == slotNum)
            {
                optionImg.enabled = true;
                optionImg.color = optionImgColor;
                Debug.Log("옵션 설정, 해당 옵션 이미지 Visible");
            }
        }
        #endregion // Handle Methods

        public void Initialize(EType type, bool bConfirmed, int slotIndex)
        {
            slotNum = slotIndex;
            this.bConfirmed = bConfirmed;
            bLock = !bConfirmed;
            lockImg.enabled = !bConfirmed;
            curType = type;

            switch (curType)
            {
                case EType.ALPHA:
                    backImg.color = new Color(1, 0.54f, 0.54f);
                    break;
                case EType.BETA:
                    backImg.color = new Color(0.54f, 0.54f, 1);
                    break;
                default:
                    backImg.color = new Color(1, 1, 1);
                    break;
            }
        }

        public void OnClicked()
        {
            OnClickedSlot?.Invoke((int)curType, slotNum);
            if (bConfirmed)
            {
                OnDisplayOptionList?.Invoke((int)curType);
            }
            else
            {
                if (bLock == true)
                {
                    Debug.Log("Lock");
                }
                else
                {
                    Debug.Log("UnLock");
                }
            }
        }
    }
}
