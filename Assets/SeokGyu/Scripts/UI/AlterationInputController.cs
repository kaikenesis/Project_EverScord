
using System;
using UnityEngine;

namespace EverScord
{
    public class AlterationInputController : UIInputController
    {
        [SerializeField] private ToggleObject popupWindow;
        private bool bActivePopupWindow;
        [SerializeField] private GameObject probabilitySheet;
        private bool bActiveProbabilitySheet;
        [SerializeField] private ToggleObject factorOptionList;
        private bool bActiveFactorOptionList;

        private void Awake()
        {
            UIFactorSlot.OnDisplayOptionList += HandleDisplayOptionList;
            UIFactorPanel.OnUnlockFactor += HandleDisplayUnlockWindow;
        }

        private void OnDestroy()
        {
            UIFactorSlot.OnDisplayOptionList -= HandleDisplayOptionList;
            UIFactorPanel.OnUnlockFactor += HandleDisplayUnlockWindow;
        }

        private void HandleDisplayOptionList(int curTypeNum)
        {
            bActiveFactorOptionList = true;
        }

        private void HandleDisplayUnlockWindow(int cost)
        {
            bActivePopupWindow = true;
        }

        protected override void OnKeyInput()
        {
            base.OnKeyInput();

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                OnKeyDownEscape();
            }
        }

        private void OnKeyDownEscape()
        {
            if(bActivePopupWindow)
            {
                popupWindow.OnDeactivateObjects();
                return;
            }

            if(bActiveProbabilitySheet)
            {
                probabilitySheet.SetActive(false);
                return;
            }

            if(bActiveFactorOptionList)
            {
                factorOptionList.OnDeactivateObjects();
                return;
            }
        }

        public void SetActivePopupWindow(bool bActive)
        {
            bActivePopupWindow = bActive;
        }

        public void OnToggleProbabilitySheet()
        {
            bActiveProbabilitySheet = !bActiveProbabilitySheet;
        }

        public void SetActiveFactorOptionList(bool bActive)
        {
            bActiveFactorOptionList = bActive;
        }
    }
}
