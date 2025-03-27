using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIRoomView : ToggleObject
    {
        [field: SerializeField] public GameObject RoomContainer;
        [field: SerializeField] public GameObject InviteButton;
        [field: SerializeField] public Button[] SingleOnlyButtons;
        [field: SerializeField] public Button[] MasterOnlyButtons;
        [field: SerializeField] public Button[] GameSettingButtons;

        private void Awake()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();


        }

        public void DisplayRoom()
        {

        }
    }
}
