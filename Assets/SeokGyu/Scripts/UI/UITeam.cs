using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITeam : MonoBehaviour
{
    [SerializeField] private int teamSize;
    [SerializeField] private int maxTeamSize;
    [SerializeField] private PhotonTeam team;
    [SerializeField] private TMP_Text teamNameText;
    [SerializeField] private Transform playerSelectionContainer;
    [SerializeField] private UIPlayerSelection playerSelectionPrefab;
    [SerializeField] private Dictionary<Player, UIPlayerSelection> playerSelections;

    public static Action<PhotonTeam> OnSwitchToTeam = delegate { };

    private void Awake()
    {
        UIDisplayTeam.OnAddPlayerToTeam += HandleAddPlayerToTeam;
        UIDisplayTeam.OnRemovePlayerFromTeam += HandleRemovePlayerFromTeam;
        PhotonRoomController.OnRoomLeft += HandleLeaveRoom;
    }

    private void OnDestroy()
    {
        UIDisplayTeam.OnAddPlayerToTeam -= HandleAddPlayerToTeam;
        UIDisplayTeam.OnRemovePlayerFromTeam -= HandleRemovePlayerFromTeam;
        PhotonRoomController.OnRoomLeft -= HandleLeaveRoom;
    }

    public void Initialize(PhotonTeam team, int teamSize)
    {
        this.team = team;
        maxTeamSize = teamSize;
        Debug.Log($"{team.Name} is added with ths size {maxTeamSize}");
        playerSelections = new Dictionary<Player, UIPlayerSelection>();
        UpdateTeamUI();

        Player[] teamMembers;
        if(PhotonTeamsManager.Instance.TryGetTeamMembers(team.Code, out teamMembers))
        {
            for (int i = 0; i < teamMembers.Length; i++)
            {
                AddPlayerToTeam(teamMembers[i]);
            }
        }
    }

    private void HandleAddPlayerToTeam(Player player, PhotonTeam team)
    {
        if (this.team.Code == team.Code)
        {
            Debug.Log($"Updating {this.team.Name} UI to add {player.NickName}");
            AddPlayerToTeam(player);
        }
    }

    private void HandleRemovePlayerFromTeam(Player player)
    {
        RemovePlayerFromTeam(player);
    }

    private void HandleLeaveRoom()
    {
        Destroy(gameObject);
    }

    private void UpdateTeamUI()
    {
        teamNameText.SetText($"{team.Name} \n {playerSelections.Count} / {maxTeamSize}");
    }

    private void AddPlayerToTeam(Player player)
    {
        UIPlayerSelection uiPlayerSelection = Instantiate(playerSelectionPrefab, playerSelectionContainer);
        uiPlayerSelection.Initialize(player);
        playerSelections.Add(player, uiPlayerSelection);
        UpdateTeamUI();
    }

    private void RemovePlayerFromTeam(Player player)
    {
        if(playerSelections.ContainsKey(player))
        {
            Debug.Log($"Updating {team.Name} UI to remove {player.NickName}");
            Destroy(playerSelections[player].gameObject);
            playerSelections.Remove(player);
            UpdateTeamUI();
        }
    }

    public void SwitchToTeam()
    {
        Debug.Log($"Trying to switch to team {team.Name}");
        if (teamSize >= maxTeamSize) return;

        Debug.Log($"Switching to team {team.Name}");
        OnSwitchToTeam?.Invoke(team);
    }
}
