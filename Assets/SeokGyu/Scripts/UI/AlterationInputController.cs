
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
            UIPopUpWindow.OnAcceptUnlock += HandleAcceptUnlock;
        }

        private void OnDestroy()
        {
            UIFactorSlot.OnDisplayOptionList -= HandleDisplayOptionList;
            UIFactorPanel.OnUnlockFactor += HandleDisplayUnlockWindow;
            UIPopUpWindow.OnAcceptUnlock -= HandleAcceptUnlock;
        }

        private void HandleDisplayOptionList(int curTypeNum)
        {
            bActiveFactorOptionList = true;
        }

        private void HandleDisplayUnlockWindow(int cost)
        {
            bActivePopupWindow = true;
        }

        private void HandleAcceptUnlock()
        {
            bActivePopupWindow = false;
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
                bActivePopupWindow = false;
                popupWindow.OnDeactivateObjects();
                return;
            }

            if(bActiveProbabilitySheet)
            {
                bActiveProbabilitySheet = false;
                probabilitySheet.SetActive(false);
                return;
            }

            if(bActiveFactorOptionList)
            {
                bActiveFactorOptionList = false;
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
