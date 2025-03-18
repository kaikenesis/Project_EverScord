using UnityEngine;

namespace EverScord
{
    public class InGameInputController : UIInputController
    {
        [SerializeField] private ToggleObject popupSetting;
        private bool bActivePopupSetting;

        protected override void OnKeyInput()
        {
            base.OnKeyInput();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnKeyDownEscape();
            }
        }

        private void OnKeyDownEscape()
        {
            if (bActivePopupSetting)
            {
                bActivePopupSetting = false;
                popupSetting.OnDeactivateObjects();
            }
            else
            {
                bActivePopupSetting = true;
                popupSetting.OnActivateObjects();
            }
        }

        public void SetActivePopupSetting(bool bActive)
        {
            bActivePopupSetting = bActive;
        }
    }
}
