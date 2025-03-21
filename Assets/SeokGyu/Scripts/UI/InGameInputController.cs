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
                popupSetting.PlayDoTween(true);
            }
            else
            {
                SoundManager.Instance.PlaySound("ButtonSound");
                bActivePopupSetting = true;
                popupSetting.PlayDoTween(false);
            }
        }

        public void SetActivePopupSetting(bool bActive)
        {
            bActivePopupSetting = bActive;
        }
    }
}
