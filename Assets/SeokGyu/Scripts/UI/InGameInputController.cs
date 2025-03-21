using UnityEngine;

namespace EverScord
{
    public class InGameInputController : BaseInputController
    {
        [SerializeField] private ToggleObject popupSetting;
        private bool bActivePopupSetting;

        private void Update()
        {
            OnKeyInput();
        }

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
                SoundManager.Instance.PlaySound("ButtonSound");
                bActivePopupSetting = false;
                popupSetting.OnDeactivateObjects();
            }
            else
            {
                SoundManager.Instance.PlaySound("ButtonSound");
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
