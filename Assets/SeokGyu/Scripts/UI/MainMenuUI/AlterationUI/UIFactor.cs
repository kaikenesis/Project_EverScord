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
        public static Action<int, int, Color, string, float> OnApplyConfirmedOption = delegate { };

        private void Awake()
        {
            UIPopUpWindow.OnAcceptUnlock += HandleAcceptUnlock;
            UIPopUpWindow.OnAcceptReroll += HandleAcceptReroll;
            UIPopUpWindow.OnApplyOption += HandleApplyOption;
            UIFactorSlot.OnClickedSlot += HandleClickedSlot;
            UIFactorOptionList.OnApplyOption += HandleApplyOption;

            Init();
        }

        private void OnDestroy()
        {
            UIPopUpWindow.OnAcceptUnlock -= HandleAcceptUnlock;
            UIPopUpWindow.OnAcceptReroll -= HandleAcceptReroll;
            UIPopUpWindow.OnApplyOption -= HandleApplyOption;
            UIFactorSlot.OnClickedSlot -= HandleClickedSlot;
            UIFactorOptionList.OnApplyOption -= HandleApplyOption;
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
            OnApplyOption?.Invoke(selectType, selectIndex, new Color(), "", 0.0f);
        }

        private void HandleApplyOption(Color newColor, string newName, float newValue)
        {
            OnApplyOption.Invoke(selectType, selectIndex, newColor, newName, newValue);
        }

        private void HandleClickedSlot(int type, int slotNum)
        {
            selectType = type;
            selectIndex = slotNum;
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
