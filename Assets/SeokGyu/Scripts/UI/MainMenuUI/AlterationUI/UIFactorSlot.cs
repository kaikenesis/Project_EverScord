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
        private string newOptionName;
        private float newOptionValue;
        private Sprite newOptionSprite;
        private Color newOptionColor;

        public bool bLock { get; private set; }
        public bool bConfirmed { get; private set; }
        public string curOptionName { get; private set; }
        public float curOptionValue { get; private set; }


        public static Action<int, int> OnClickedSlot = delegate { };
        public static Action<int> OnDisplayOptionList = delegate { };
        public static Action<string, float, string, float> OnRequestUpdateInfo = delegate { };
        public static Action<Color, string, float, string, float> OnRequestApplyOption = delegate { };
        public static Action<int> OnDecreaseMoney = delegate { };

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
            if (bLock == false || curTypeNum != typeNum || this.slotNum != slotNum) return;

            int cost = GameManager.Instance.CostDatas.SlotCostDatas[slotNum - 1].Unlock;
            GameManager.Instance.PlayerData.DecreaseMoney(cost);
            OnDecreaseMoney?.Invoke(GameManager.Instance.PlayerData.money);
            bLock = false;
            lockImg.enabled = false;
        }

        private void HandleRequestReroll(int typeNum, int slotNum)
        {
            if (bLock == true || curTypeNum != typeNum || this.slotNum != slotNum) return;

            int cost = GameManager.Instance.CostDatas.SlotCostDatas[slotNum - 1].Reroll;
            GameManager.Instance.PlayerData.DecreaseMoney(cost);
            OnDecreaseMoney?.Invoke(GameManager.Instance.PlayerData.money);

            FactorData datas = GameManager.Instance.FactorDatas[typeNum];

            int randomOptionNum = UnityEngine.Random.Range(0, datas.OptionDatas.Length);

            newOptionSprite = datas.OptionDatas[randomOptionNum].SourceImg;
            newOptionColor = datas.OptionDatas[randomOptionNum].ImgColor;
            newOptionName = datas.OptionDatas[randomOptionNum].Name;

            int randomValueNum = UnityEngine.Random.Range(0, datas.OptionDatas[randomOptionNum].Values.Length);
            newOptionValue = datas.OptionDatas[randomOptionNum].Values[randomValueNum];

            OnRequestApplyOption?.Invoke(newOptionColor, newOptionName, newOptionValue, curOptionName, curOptionValue);
        }

        private void HandleApplyOption(int typeNum, int slotNum, string newName, float newValue, Sprite newSourceImg, Color newColor)
        {
            if (bLock == true || curTypeNum != typeNum || this.slotNum != slotNum) return;

            if (newName == "")
            {
                OnRequestUpdateInfo?.Invoke(newOptionName, newOptionValue, curOptionName, curOptionValue);

                optionImg.enabled = true;
                optionImg.sprite = newOptionSprite;
                optionImg.color = newOptionColor;
                curOptionName = newOptionName;
                curOptionValue = newOptionValue;
            }
            else
            {
                OnRequestUpdateInfo?.Invoke(newName, newValue, curOptionName, curOptionValue);

                optionImg.enabled = true;
                optionImg.sprite = newSourceImg;
                optionImg.color = newColor;
                curOptionName = newName;
                curOptionValue = newValue;
            }
        }
        #endregion // Handle Methods

        public void Initialize(int typeNum, bool bConfirmed, int slotIndex)
        {
            UIFactorSlot slotData = GameManager.Instance.PlayerAlterationData[typeNum].slots[slotIndex];
            slotNum = slotIndex;
            this.bConfirmed = bConfirmed;
            lockImg.sprite = GameManager.Instance.FactorDatas[typeNum].LockedSourceImg;
            curTypeNum = typeNum;

            if (slotData != null)
            {
                bLock = slotData.bLock;
                lockImg.enabled = slotData.bLock;
                if(bLock == false)
                {
                    curOptionName = slotData.curOptionName;
                    curOptionValue = slotData.curOptionValue;
                    optionImg.enabled = slotData.optionImg.enabled;
                    optionImg.sprite = slotData.optionImg.sprite;
                    optionImg.color = slotData.newOptionColor;
                }
            }
            else
            {
                bLock = !bConfirmed;
                lockImg.enabled = !bConfirmed;
            }

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
