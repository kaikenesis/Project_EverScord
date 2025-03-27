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
            for (int i = 0; i < 3; i++)
            {
                UIRoomPlayer uiRoomPlayer = Instantiate(model.UIRoomPlayer, view.RoomContainer.transform);
                uiRoomPlayers[i] = uiRoomPlayer;
            }
        }

        #region Handle Methods
        private void HandleDisplayPlayers(List<string> players, List<Tuple<int, int>> typeDatas)
        {
            int count = GameManager.Instance.GameMode.maxPlayer;
            for(int i = 0; i< count; i++)
            {
                if(i < players.Count)
                {
                    bool bMaster = false;
                    if (PhotonNetwork.MasterClient.NickName == players[i])
                        bMaster = true;

                    uiRoomPlayers[i].Initialize(players[i], bMaster, typeDatas[i].Item1, typeDatas[i].Item2);
                    uiRoomPlayers[i].gameObject.SetActive(true);
                }
                else
                {
                    uiRoomPlayers[i].gameObject.SetActive(false);
                }
            }

            if (players.Count < count)
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
            view.SetActiveMasterButton(PhotonNetwork.IsMasterClient);

            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                view.SetActiveSingleButton(false);
            }
            else
            {
                view.SetActiveSingleButton(true);
            }
        }

        private void HandleUpdateUI(bool bActive)
        {
            SetActiveGameUI(bActive);
        }
        #endregion // Handle Methods

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

            view.SetActiveGameSettingButton(bActive);
        }
    }
}
