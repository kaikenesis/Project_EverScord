using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerSelection : MonoBehaviour
{
    [SerializeField] private TMP_Text userNameText;
    [SerializeField] private Player owner;

    public Player Owner
    {
        get { return owner; }
        private set { owner = value; }
    }

    public void Initialize(Player player)
    {
        Debug.Log($"Player Selection Init {player.NickName}");
        Owner = player;
        SetupPlayerSelection();
    }

    private void SetupPlayerSelection()
    {
        userNameText.SetText(owner.NickName);
    }
}
