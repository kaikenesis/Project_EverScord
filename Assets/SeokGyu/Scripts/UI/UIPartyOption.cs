using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class UIPartyOption : MonoBehaviour
    {
        [SerializeField] private GameObject exileButton;
        [SerializeField] private GameObject exitButton;
        private string nickName;
        private RectTransform rect;

        public static Action OnClickedExit = delegate { };
        public static Action<string> OnClickedExile = delegate { };

        private void Awake()
        {
            UIRoomPlayer.OnDisplayPartyOption += HandleDisplayPartyOption;

            nickName = "";
            rect = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            UIRoomPlayer.OnDisplayPartyOption -= HandleDisplayPartyOption;
        }

        private void HandleDisplayPartyOption(bool bVisible, string targetName, Vector2 mousePos)
        {
            if (PhotonNetwork.InRoom == false) return;
            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1) return;

            gameObject.SetActive(bVisible);
            if (bVisible == true)
            {
                OnDisplayPanel(mousePos);
                OnDisplayButtons(targetName);
            }
        }

        private void OnDisplayButtons(string targetName)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                nickName = targetName;

                if (targetName == PhotonNetwork.NickName)
                {
                    exitButton.SetActive(true);
                    exileButton.SetActive(false);
                }
                else
                {
                    exitButton.SetActive(false);
                    exileButton.SetActive(true);
                }
            }
            else
            {
                if (targetName == PhotonNetwork.NickName)
                {
                    exitButton.SetActive(true);
                    exileButton.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private void OnDisplayPanel(Vector2 mousePos)
        {
            if (Screen.width - mousePos.x <= rect.sizeDelta.x)
            {
                rect.pivot = new Vector2(1.0f, 1.0f);
            }
            else
            {
                rect.pivot = new Vector2(0.0f, 1.0f);
            }
            transform.position = mousePos;
        }

        private bool IsPlayerInCurrentRoom(string targetName)
        {
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            int key = 0;
            for (int i = 0; i < players.Count; key++)
            {
                if(players.ContainsKey(key))
                {
                    if (players[key].NickName == targetName)
                        return true;
                    i++;
                }
            }

            return false;
        }

        public void ClickedExit()
        {
            OnClickedExit?.Invoke();
            gameObject.SetActive(false);
        }

        public void ClickedExile()
        {
            if (IsPlayerInCurrentRoom(nickName))
                OnClickedExile?.Invoke(nickName);
            gameObject.SetActive(false);
        }
    }
}