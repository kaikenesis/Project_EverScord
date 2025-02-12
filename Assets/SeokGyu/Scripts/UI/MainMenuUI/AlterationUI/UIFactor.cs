using System;
using UnityEngine;

namespace EverScord
{
    public class UIFactor : MonoBehaviour
    {
        [SerializeField] private FactorPanel[] panels;
        [SerializeField] private Transform containor;
        [SerializeField] private GameObject factorPanel;
        [SerializeField] private GameObject optionPanel;
        private int selectType;
        private int selectIndex;

        public static Action<int, int> OnRequestUnlock = delegate { };
        public static Action<int, int> OnRequestReroll = delegate { };
        public static Action<int, int, Color, string, float> OnApplyOption = delegate { };

        private void Awake()
        {
            UIPopUpWindow.OnAcceptUnlock += HandleAcceptUnlock;
            UIPopUpWindow.OnAcceptReroll += HandleAcceptReroll;
            UIFactorSlot.OnClickedSlot += HandleClickedSlot;
            UIFactorOptionList.OnRequestApplyOption += HandleRequestApplyOption;
            UIFactorSlot.OnRequestApplyOption += HandleRequestApplyOption;

            Init();
        }

        private void OnDestroy()
        {
            UIPopUpWindow.OnAcceptUnlock -= HandleAcceptUnlock;
            UIPopUpWindow.OnAcceptReroll -= HandleAcceptReroll;
            UIFactorSlot.OnClickedSlot -= HandleClickedSlot;
            UIFactorOptionList.OnRequestApplyOption -= HandleRequestApplyOption;
            UIFactorSlot.OnRequestApplyOption -= HandleRequestApplyOption;
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

        private void HandleClickedSlot(int type, int slotNum)
        {
            selectType = type;
            selectIndex = slotNum;
        }

        private void HandleRequestApplyOption(Color optionImgColor, string name, float value)
        {
            OnApplyOption?.Invoke(selectType, selectIndex, optionImgColor, name, value);
        }
        #endregion // Handle Methods

        private void Init()
        {
            for (int i = 0; i < panels.Length; i++)
            {
                UIFactorPanel panel = Instantiate(factorPanel, containor).GetComponent<UIFactorPanel>();
                panel.Initialize((int)panels[i].Type, panels[i].SlotCount, panels[i].ConfirmedCount);
            }
        }

        [System.Serializable]
        public class FactorPanel
        {
            public enum EType
            {
                ALPHA,
                BETA,
                MAX
            }

            [SerializeField] private EType type;
            [SerializeField] private int slotCount;
            [SerializeField] private int confirmedCount;

            public EType Type
            {
                get { return type; }
                private set { type = value; }
            }

            public int SlotCount
            {
                get { return slotCount; }
                private set {  slotCount = value; }
            }

            public int ConfirmedCount
            {
                get { return confirmedCount; }
                private set { confirmedCount = value; }
            }
        }
    }
}
