using EverScord.Character;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class UIDisplayParty : MonoBehaviour
    {
        [SerializeField] private GameObject portrait;
        [SerializeField] private Transform myPortrait;
        private List<GameObject> portraits = new List<GameObject>();

        private void Awake()
        {
            CharacterControl.OnPhotonViewListUpdated += HandlePhotonViewListUpdated;
            CharacterControl.OnCheckAlive += HandleCheckAlive;
        }

        private void OnDestroy()
        {
            CharacterControl.OnPhotonViewListUpdated -= HandlePhotonViewListUpdated;
            CharacterControl.OnCheckAlive -= HandleCheckAlive;
        }

        private void HandlePhotonViewListUpdated()
        {
            int playerCount = GameManager.Instance.PlayerDict.Count;
            int i = 0;

            Debug.Log(GameManager.Instance.PlayerDict.Count);

            foreach (CharacterControl player in GameManager.Instance.PlayerDict.Values)
            {
                int viewID = player.CharacterPhotonView.ViewID;
                Debug.Log($"{player.gameObject} : {viewID}");

                if (portraits.Count < playerCount)
                {
                    GameObject obj;
                    if (i != 0)
                    {
                        obj = Instantiate(portrait, transform);
                    }
                    else
                    {
                        obj = Instantiate(portrait, myPortrait);
                        obj.transform.localPosition = Vector3.zero;
                    }
                    
                    portraits.Add(obj);
                    Debug.Log($"{player.CharacterType} , {player.CharacterJob}");

                    obj.GetComponent<UIPortrait>().Initialize(player.CharacterType, viewID);
                    obj.GetComponentInChildren<UIJobIcon>().Initialize(player.CharacterJob, viewID);
                }
                ++i;
            }
        }

        private void HandleCheckAlive(int pvID, bool bDead, Vector3 position)
        {
            for (int i = 0; i < portraits.Count; i++)
            {
                portraits[i].GetComponent<UIPortrait>().UpdatePortrait(pvID, bDead);
            }
        }
    }
}
