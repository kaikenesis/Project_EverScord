using EverScord.Character;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EverScord
{
    public class UIDisplayParty : MonoBehaviour
    {
        [SerializeField] private GameObject portrait;

        private void Awake()
        {
            CharacterControl.OnPhotonViewListUpdated += HandlePhotonViewListUpdated;
        }

        private void OnDestroy()
        {
            CharacterControl.OnPhotonViewListUpdated -= HandlePhotonViewListUpdated;
        }

        private void HandlePhotonViewListUpdated()
        {
            List<PhotonView> list = GameManager.Instance.playerPhotonViews;
            Debug.Log(list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                Instantiate(portrait, transform);
                Debug.Log($"{list[i].gameObject} : {list[i].ViewID}");
                //if (list[i].ViewID == )
                //    continue;
            }
        }
    }
}
