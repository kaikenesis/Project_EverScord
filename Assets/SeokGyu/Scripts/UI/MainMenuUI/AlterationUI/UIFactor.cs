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

        public static Action<int,int> OnRequestUnlock = delegate { };
        public static Action<int,int,Color> OnApplyOption = delegate { };

        private void Awake()
        {
            UIPopUpWindow.OnAcceptUnlock += HandleAcceptUnlock;
            UIFactorSlot.OnClickedSlot += HandleClickedSlot;
            UIFactorOptionList.OnRequestApplyOption += HandleRequestApplyOption;

            Init();
        }

        private void OnDestroy()
        {
            UIPopUpWindow.OnAcceptUnlock -= HandleAcceptUnlock;
            UIFactorSlot.OnClickedSlot -= HandleClickedSlot;
            UIFactorOptionList.OnRequestApplyOption -= HandleRequestApplyOption;
        }

        #region Handle Methods
        private void HandleAcceptUnlock()
        {
            OnRequestUnlock?.Invoke(selectType, selectIndex);
        }

        private void HandleClickedSlot(int type, int slotNum)
        {
            selectType = type;
            selectIndex = slotNum;
        }

        private void HandleRequestApplyOption(Color optionImgColor, string name, float value)
        {
            OnApplyOption?.Invoke(selectType, selectIndex, optionImgColor);
        }
        #endregion // Handle Methods

        private void Init()
        {
            for (int i = 0; i < panels.Length; i++)
            {
                UIFactorPanel panel = Instantiate(factorPanel, containor).GetComponent<UIFactorPanel>();
                panel.Initialize(panels[i].Type, panels[i].SlotCount, panels[i].ConfirmedCount);
            }
        }

        [System.Serializable]
        public class FactorPanel
        {
            [SerializeField] private UIFactorSlot.EType type;
            [SerializeField] private int slotCount;
            [SerializeField] private int confirmedCount;

            public UIFactorSlot.EType Type
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
