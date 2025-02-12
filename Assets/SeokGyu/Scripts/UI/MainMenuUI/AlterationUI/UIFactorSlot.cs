using System;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIFactorSlot : MonoBehaviour
    {
        [SerializeField] private Image backImg;
        [SerializeField] private Image lockImg;
        [SerializeField] private Image optionImg;
        private int curTypeNum;
        private int slotNum;

        public bool bLock { get; private set; }
        public bool bConfirmed { get; private set; }
        public string curOptionName { get; private set; }
        public float curOptionValue { get; private set; }

        public static Action<int, int> OnClickedSlot = delegate { };
        public static Action<int> OnDisplayOptionList = delegate { };
        public static Action<string, string, float, float> OnRequestUpdateInfo = delegate { };
        public static Action<Color, string, float> OnRequestApplyOption = delegate { };

        private void Awake()
        {
            UIFactor.OnRequestUnlock += HandleRequestUnlock;
            UIFactor.OnRequestReroll += HandleRequestReroll;
            UIFactor.OnApplyOption += HandleApplyOption;

            bLock = true;
            optionImg.enabled = false;
        }

        private void OnDestroy()
        {
            UIFactor.OnRequestUnlock -= HandleRequestUnlock;
            UIFactor.OnRequestReroll -= HandleRequestReroll;
            UIFactor.OnApplyOption -= HandleApplyOption;
        }

        #region Handle Methods
        private void HandleRequestUnlock(int typeNum, int slotNum)
        {
            if (bLock == true && curTypeNum == typeNum && this.slotNum == slotNum)
            {
                int cost = GameManager.Instance.CostDatas.SlotCostDatas[slotNum - 1].Unlock;
                GameManager.Instance.userData.DecreaseMoney(cost);
                bLock = false;
                lockImg.enabled = false;
            }
        }

        private void HandleRequestReroll(int typeNum, int slotNum)
        {
            if (bLock == false && curTypeNum == typeNum && this.slotNum == slotNum)
            {
                int cost = GameManager.Instance.CostDatas.SlotCostDatas[slotNum - 1].Reroll;
                GameManager.Instance.userData.DecreaseMoney(cost);

                FactorDatas datas = GameManager.Instance.FactorDatas[typeNum];

                int randomOptionNum = UnityEngine.Random.Range(0, datas.OptionDatas.Length - 1);

                Color imgColor = datas.OptionDatas[randomOptionNum].ImgColor;
                string name = datas.OptionDatas[randomOptionNum].Name;

                int randomValueNum = UnityEngine.Random.Range(0, datas.OptionDatas[randomOptionNum].Values.Length - 1);
                float value = datas.OptionDatas[randomOptionNum].Values[randomValueNum];

                OnRequestApplyOption?.Invoke(imgColor, name, value);
            }
        }

        private void HandleApplyOption(int typeNum, int slotNum, Color optionImgColor, string newName, float newValue)
        {
            if (bLock == false && curTypeNum == typeNum && this.slotNum == slotNum)
            {
                OnRequestUpdateInfo?.Invoke(curOptionName, newName, curOptionValue, newValue);
                
                optionImg.enabled = true;
                optionImg.color = optionImgColor;
                curOptionName = newName;
                curOptionValue = newValue;
            }
        }
        #endregion // Handle Methods

        public void Initialize(int typeNum, bool bConfirmed, int slotIndex)
        {
            slotNum = slotIndex;
            this.bConfirmed = bConfirmed;
            bLock = !bConfirmed;
            lockImg.enabled = !bConfirmed;
            curTypeNum = typeNum;

            switch (curTypeNum)
            {
                case 0:
                    backImg.color = new Color(1, 0.54f, 0.54f);
                    break;
                case 1:
                    backImg.color = new Color(0.54f, 0.54f, 1);
                    break;
                default:
                    backImg.color = new Color(1, 1, 1);
                    break;
            }
        }

        public void OnClicked()
        {
            OnClickedSlot?.Invoke(curTypeNum, slotNum);
            if (bConfirmed)
            {
                OnDisplayOptionList?.Invoke(curTypeNum);
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
