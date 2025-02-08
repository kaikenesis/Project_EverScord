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
        private EType curType;
        private bool bLock = true;
        private bool bConfirmed;
        private int slotNum;

        public static Action<bool, bool, int, int> OnClickedSlot = delegate { };
        public static Action<int> OnDisplayOptionList = delegate { };

        private void Awake()
        {
            UIFactor.OnRequestUnlock += HandleRequestUnlock;
            UIFactor.OnApplyOption += HandleApplyOption;
            
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
                bLock = false;
                lockImg.enabled = false;
            }
        }

        private void HandleApplyOption(int type, int slotNum)
        {
            if (bLock == false && curType == (EType)type && this.slotNum == slotNum)
            {
                bConfirmed = false;
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
            if (bConfirmed)
            {
                OnDisplayOptionList?.Invoke((int)curType);
            }
            else
            {
                if (bLock == true)
                {
                    OnClickedSlot?.Invoke(bConfirmed, bLock, (int)curType, slotNum);
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
