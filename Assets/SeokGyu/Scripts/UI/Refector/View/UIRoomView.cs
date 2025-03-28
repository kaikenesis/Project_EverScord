using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIRoomView : ToggleObject
    {
        [field: SerializeField] public GameObject RoomContainer { get; private set; }
        [field: SerializeField] public GameObject InviteButton { get; private set; }
        [field: SerializeField] public Button[] SingleOnlyButtons { get; private set; }
        [field: SerializeField] public Button[] MasterOnlyButtons { get; private set; }
        [field: SerializeField] public Button[] GameSettingButtons { get; private set; }

        private void Awake()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void SetActiveMasterButton(bool bActive)
        {
            for (int i = 0; i < MasterOnlyButtons.Length; i++)
            {
                MasterOnlyButtons[i].interactable = bActive;
            }
        }

        public void SetActiveSingleButton(bool bActive)
        {
            for (int i = 0; i < SingleOnlyButtons.Length; i++)
            {
                SingleOnlyButtons[i].interactable = bActive;
            }
        }

        public void SetActiveGameSettingButton(bool bActive)
        {
            for (int i = 0; i < GameSettingButtons.Length; i++)
            {
                GameSettingButtons[i].interactable = bActive;
            }
        }
    }
}
