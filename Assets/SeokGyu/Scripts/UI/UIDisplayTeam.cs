using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using EverScord;

public class UIDisplayTeam : MonoBehaviour
{
    [SerializeField] private UITeam uiTeamPrefab;
    [SerializeField] private List<UITeam> uiTeams;
    [SerializeField] private Transform teamContainer;

    public static Action<Player, PhotonTeam> OnAddPlayerToTeam = delegate { };
    public static Action<Player> OnRemovePlayerFromTeam = delegate { };

    private void Awake()
    {
        PhotonTeamController.OnCreateTeams += HandleCreateTeams;
        PhotonTeamController.OnSwitchTeam += HandleSwitchTeam;
        PhotonTeamController.OnRemovePlayer += HandleRemovePlayer;
        PhotonTeamController.OnClearTeams += HandleClearTeams;
        uiTeams = new List<UITeam>();
    }

    private void OnDestroy()
    {
        PhotonTeamController.OnCreateTeams -= HandleCreateTeams;
        PhotonTeamController.OnSwitchTeam -= HandleSwitchTeam;
        PhotonTeamController.OnRemovePlayer -= HandleRemovePlayer;
        PhotonTeamController.OnClearTeams -= HandleClearTeams;
    }

    private void HandleCreateTeams(List<PhotonTeam> teams, GameMode gameMode)
    {
        for (int i = 0; i < teams.Count; i++)
        {
            UITeam uiTeam = Instantiate(uiTeamPrefab, teamContainer);
            uiTeam.Initialize(teams[i], gameMode.TeamSize);
            uiTeams.Add(uiTeam);
        }
    }

    private void HandleSwitchTeam(Player player, PhotonTeam newTeam)
    {
        Debug.Log($"Updating UI to move {player.NickName} to {newTeam.Name}");

        OnRemovePlayerFromTeam?.Invoke(player);
        OnAddPlayerToTeam?.Invoke(player, newTeam);
    }

    private void HandleRemovePlayer(Player player)
    {
        OnRemovePlayerFromTeam?.Invoke(player);
    }

    private void HandleClearTeams()
    {
        for (int i = 0; i < uiTeams.Count; i++)
        {
            Destroy(uiTeams[i].gameObject);
        }
        uiTeams.Clear();
    }
}
