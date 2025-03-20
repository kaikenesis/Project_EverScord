using UnityEngine;

namespace EverScord
{
    public class UIMessageBox : ToggleObject
    {
        [SerializeField] private PopUpWindowData model;
        [SerializeField] private UIDisplayMessageBox view;

        protected override void Initialize()
        {
            base.Initialize();

            PhotonRoomController.OnCannotGameStart += HandleCannotGameStart;

            view.yesButton.onClick.AddListener(Deactive);
        }

        private void OnDestroy()
        {
            PhotonRoomController.OnCannotGameStart -= HandleCannotGameStart;
        }

        private void HandleCannotGameStart()
        {
            SetMessageBox();
        }

        public void SetMessageBox()
        {
            string titleText = model.Messages[(int)view.type - 1].TitleText;
            string mainMsg = model.Messages[(int)view.type - 1].MainMessage;

            view.SetMessageText(titleText, mainMsg);

            PlayDoTween(false);
        }

        private void Deactive()
        {
            PlayDoTween(true);
        }
    }
}

