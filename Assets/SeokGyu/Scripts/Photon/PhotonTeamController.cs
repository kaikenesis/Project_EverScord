using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;

namespace EverScord
{
    public class PhotonTeamController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private List<PhotonTeam> roomTeams;
        [SerializeField] private int teamSize;
        [SerializeField] private PhotonTeam priorTeam;

        public static Action<List<PhotonTeam>, GameMode> OnCreateTeams = delegate { };
        public static Action<Player, PhotonTeam> OnSwitchTeam = delegate { };
        public static Action<Player> OnRemovePlayer = delegate { };
        public static Action OnClearTeams = delegate { };

        private void Awake()
        {
            UITeam.OnSwitchToTeam += HandleSwitchTeam;
            PhotonRoomController.OnJoinRoom += HandleCreateTeams;
            PhotonRoomController.OnRoomLeft += HandleLeaveRoom;
            PhotonRoomController.OnOtherPlayerLeftRoom += HandleOtherPlayerLeftRoom;

            roomTeams = new List<PhotonTeam>();
        }

        private void OnDestroy()
        {
            UITeam.OnSwitchToTeam -= HandleSwitchTeam;
            PhotonRoomController.OnJoinRoom -= HandleCreateTeams;
            PhotonRoomController.OnRoomLeft -= HandleLeaveRoom;
            PhotonRoomController.OnOtherPlayerLeftRoom -= HandleOtherPlayerLeftRoom;
        }

        #region Handle Methods
        private void HandleSwitchTeam(PhotonTeam newTeam)
        {
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam() == null)
            {
                priorTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
                PhotonNetwork.LocalPlayer.JoinTeam(newTeam);
            }
            else if(CanSwitchToTeam(newTeam))
            {
                priorTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
                PhotonNetwork.LocalPlayer.SwitchTeam(newTeam);
            }
        }
        private void HandleCreateTeams(GameMode gameMode)
        {
            CreateTeams(gameMode);

            OnCreateTeams?.Invoke(roomTeams, gameMode);

            AutoAssignPlayerToTeam(PhotonNetwork.LocalPlayer, gameMode);
        }
        private void HandleLeaveRoom()
        {
            PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
            roomTeams.Clear();
            teamSize = 0;
            OnClearTeams?.Invoke();
        }
        private void HandleOtherPlayerLeftRoom(Player otherPlayer)
        {
            OnRemovePlayer?.Invoke(otherPlayer);
        }
        #endregion

        #region Private Methods
        private void CreateTeams(GameMode gameMode)
        {
            teamSize = gameMode.TeamSize;
            int numberOfTeams = gameMode.MaxPlayers;
            if(gameMode.HasTeams)
            {
                numberOfTeams = gameMode.MaxPlayers / gameMode.TeamSize;
            }

            for (int i = 1; i <= numberOfTeams; i++)
            {
                roomTeams.Add(new PhotonTeam { Name = $"Team {i}", Code = (byte)i });
            }
        }

        private bool CanSwitchToTeam(PhotonTeam newTeam)
        {
            bool canSwitch = false;

            if(PhotonNetwork.LocalPlayer.GetPhotonTeam().Code != newTeam.Code)
            {
                Player[] players = null;
                if(PhotonTeamsManager.Instance.TryGetTeamMembers(newTeam.Code, out players))
                {
                    if(players.Length < teamSize)
                    {
                        canSwitch = true;
                    }
                    else
                    {
                        Debug.Log($"{newTeam.Name} is full");
                    }
                }
            }
            else
            {
                Debug.Log($"You are already on the team {newTeam.Name}");
            }

            return canSwitch;
        }

        private void AutoAssignPlayerToTeam(Player player, GameMode gameMode)
        {
            int count = roomTeams.Count;
            for (int i = 0; i < count; i++)
            {
                int teamPlayerCount = PhotonTeamsManager.Instance.GetTeamMembersCount(roomTeams[i].Code);

                if(teamPlayerCount < gameMode.TeamSize)
                {
                    Debug.Log($"Auto assigned {player.NickName} to {roomTeams[i].Name}");
                    if(player.GetPhotonTeam() == null)
                    {
                        player.JoinTeam(roomTeams[i].Code);
                    }
                    else if(player.GetPhotonTeam().Code != roomTeams[i].Code)
                    {
                        player.SwitchTeam(roomTeams[i].Code);
                    }
                    break;
                }
            }
        }
        #endregion

        #region Photon Callback Methods
        // Player가 Team을 Join, Switch, Left했을때 호출되는 콜백함수
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            object teamCodeObject;

            if(changedProps.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamCodeObject))
            {
                if (teamCodeObject == null) return;

                byte teamCode = (byte)teamCodeObject;

                PhotonTeam newTeam;
                if(PhotonTeamsManager.Instance.TryGetTeamByCode(teamCode, out newTeam))
                {
                    Debug.Log($"Switching {targetPlayer.NickName} to new team {newTeam.Name}");
                    OnSwitchTeam?.Invoke(targetPlayer, newTeam);
                }
            }
        }
        #endregion
    }
}
