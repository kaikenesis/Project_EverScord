using System;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class UIFactor : MonoBehaviour
    {
        [SerializeField] private Transform containor;
        [SerializeField] private GameObject factorPanel;
        [SerializeField] private GameObject optionPanel;
        private int selectType;
        private int selectIndex;

        public static Action<int, int> OnRequestUnlock = delegate { };
        public static Action<int, int> OnRequestReroll = delegate { };
        public static Action<int, int, string, float, Sprite, Color> OnApplyOption = delegate { };

        private void Awake()
        {
            UIPopUpWindow.OnAcceptUnlock += HandleAcceptUnlock;
            UIPopUpWindow.OnAcceptReroll += HandleAcceptReroll;
            UIPopUpWindow.OnApplyOption += HandleApplyOption;
            UIFactorSlot.OnClickedSlot += HandleClickedSlot;
            UIFactorOption.OnSelectOption += HandleSelectOption;

            Init();
        }

        private void OnDestroy()
        {
            UIPopUpWindow.OnAcceptUnlock -= HandleAcceptUnlock;
            UIPopUpWindow.OnAcceptReroll -= HandleAcceptReroll;
            UIPopUpWindow.OnApplyOption -= HandleApplyOption;
            UIFactorSlot.OnClickedSlot -= HandleClickedSlot;
            UIFactorOption.OnSelectOption -= HandleSelectOption;
        }

        #region Handle Methods
        private void HandleAcceptUnlock()
        {
            OnRequestUnlock?.Invoke(selectType, selectIndex);
        }

        private void HandleAcceptReroll()
        {
            OnRequestReroll?.Invoke(selectType, selectIndex);
        }

        private void HandleApplyOption()
        {
            OnApplyOption?.Invoke(selectType, selectIndex, "", 0.0f, null, new Color());
        }

        private void HandleClickedSlot(int type, int slotNum)
        {
            selectType = type;
            selectIndex = slotNum;
        }

        private void HandleSelectOption(int typeNum, int optionNum, float value)
        {
            FactorData datas = GameManager.Instance.FactorDatas[typeNum];
            string optionName = datas.OptionDatas[optionNum].Name;
            Sprite sourceImg = datas.OptionDatas[optionNum].SourceImg;
            Color optionImgColor = datas.OptionDatas[optionNum].ImgColor;

            AlterationData.PanelData panelData = GameManager.Instance.PlayerAlterationData.PanelDatas[typeNum];
            panelData.OptionNum[selectIndex] = optionNum;
            panelData.Value[selectIndex] = value;

            OnApplyOption?.Invoke(selectType, selectIndex, optionName, value, sourceImg, optionImgColor);
        }
        #endregion // Handle Methods

        private void Init()
        {
            int count = GameManager.Instance.FactorDatas.Length;
            for (int i = 0; i < count; i++)
            {
                FactorData data = GameManager.Instance.FactorDatas[i];
                UIFactorPanel panel = Instantiate(factorPanel, containor).GetComponent<UIFactorPanel>();
                panel.Initialize((int)data.Type, data.SlotCount, data.ConfirmedCount);
            }
        }
    }
}
