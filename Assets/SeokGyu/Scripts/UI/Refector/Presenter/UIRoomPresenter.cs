using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class UIRoomPresenter : MonoBehaviour
    {
        [SerializeField] private UIRoomModel model;
        [SerializeField] private UIRoomView view;

        private UIRoomPlayer[] uiRoomPlayers;
        private bool bFull = false;

        public static Action OnVisibleObject = delegate { };

        private void Awake()
        {
            PhotonRoomController.OnDisplayPlayers += HandleDisplayPlayers;
            PhotonRoomController.OnUpdateRoom += HandleUpdateRoom;
            PhotonMatchController.OnUpdateUI += HandleUpdateUI;

            Initialize();
        }

        private void OnDestroy()
        {
            PhotonRoomController.OnDisplayPlayers -= HandleDisplayPlayers;
            PhotonRoomController.OnUpdateRoom -= HandleUpdateRoom;
            PhotonMatchController.OnUpdateUI -= HandleUpdateUI;
        }

        private void Initialize()
        {
            uiRoomPlayers = new UIRoomPlayer[3];
            for(int i = 0;i<3;i++)
            {
                UIRoomPlayer uiRoomPlayer = Instantiate(model.UIRoomPlayer, view.RoomContainer.transform);
                uiRoomPlayers[i] = uiRoomPlayer;
            }
        }

        #region Handle Methods
        private void HandleDisplayPlayers(List<string> players, List<Tuple<int, int>> typeDatas)
        {
            int i = 0;
            for (i = 0; i < players.Count; i++)
            {
                bool bMaster = false;
                if (PhotonNetwork.MasterClient.NickName == players[i])
                    bMaster = true;

                uiRoomPlayers[i].Initialize(players[i], bMaster, typeDatas[i].Item1, typeDatas[i].Item2);
                uiRoomPlayers[i].gameObject.SetActive(true);
            }

            for (; i < 3; i++)
            {
                uiRoomPlayers[i].gameObject.SetActive(false);
            }

            if (players.Count < 3)
            {
                view.InviteButton.SetActive(PhotonNetwork.IsMasterClient);
                bFull = false;
            }
            else
            {
                view.InviteButton.SetActive(false);
                bFull = true;
            }
        }

        private void HandleUpdateRoom()
        {
            SetActiveMasterButton(PhotonNetwork.IsMasterClient);

            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                SetActiveSingleButton(false);
            }
            else
            {
                SetActiveSingleButton(true);
            }
        }

        private void HandleUpdateUI(bool bActive)
        {
            SetActiveGameUI(bActive);
        }
        #endregion // Handle Methods

        private void SetActiveMasterButton(bool bActive)
        {
            for (int i = 0; i < view.MasterOnlyButtons.Length; i++)
            {
                view.MasterOnlyButtons[i].interactable = bActive;
            }
        }

        private void SetActiveSingleButton(bool bActive)
        {
            for (int i = 0; i < view.SingleOnlyButtons.Length; i++)
            {
                view.SingleOnlyButtons[i].interactable = bActive;
            }
        }

        private void SetActiveGameUI(bool bActive)
        {
            // 파티장만 초대버튼 활성화/비활성화
            // 나머지 버튼들은 비활성화
            if (view.InviteButton.activeSelf == false && bFull == false)
            {
                view.InviteButton.SetActive(PhotonNetwork.IsMasterClient);
            }
            else if (view.InviteButton.activeSelf == true)
            {
                view.InviteButton.SetActive(false);
            }

            for (int i = 0; i < view.GameSettingButtons.Length; i++)
            {
                view.GameSettingButtons[i].interactable = bActive;
            }
        }
    }
}
