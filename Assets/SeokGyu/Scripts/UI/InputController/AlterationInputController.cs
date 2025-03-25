
using DG.Tweening;
using System;
using UnityEngine;

namespace EverScord
{
    public class AlterationInputController : BaseInputController
    {
        [SerializeField] private ToggleObject popupWindow;
        private bool bActivePopupWindow;
        [SerializeField] private ToggleObject probabilitySheet;
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

        private void Update()
        {
            OnKeyInput();
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
                SoundManager.Instance.PlaySound("ButtonSound");
                bActivePopupWindow = false;
                popupWindow.PlayDoTween(true);
                return;
            }

            if(bActiveProbabilitySheet)
            {
                SoundManager.Instance.PlaySound("ButtonSound");
                bActiveProbabilitySheet = false;
                probabilitySheet.PlayDoTween(true);
                return;
            }

            if(bActiveFactorOptionList)
            {
                SoundManager.Instance.PlaySound("ButtonSound");
                bActiveFactorOptionList = false;
                factorOptionList.PlayDoTween(true);
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
