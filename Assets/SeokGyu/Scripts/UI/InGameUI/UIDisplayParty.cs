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
            List<PhotonView> list = GameManager.Instance.playerPhotonViews;
            Debug.Log(list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                Debug.Log($"{list[i].gameObject} : {list[i].ViewID}");

                if (portraits.Count < list.Count)
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

                    CharacterControl characterControl = list[i].gameObject.GetComponent<CharacterControl>();
                    Debug.Log($"{characterControl.CharacterType} , {characterControl.CharacterJob}");

                    obj.GetComponent<UIPortrait>().Initialize(characterControl.CharacterType, list[i].ViewID);
                    obj.GetComponentInChildren<UIJobIcon>().Initialize(characterControl.CharacterJob, list[i].ViewID);
                }
            }
        }

        private void HandleCheckAlive(int pvID, bool bDead)
        {
            for (int i = 0; i < portraits.Count; i++)
            {
                portraits[i].GetComponent<UIPortrait>().UpdatePortrait(pvID, bDead);
            }
        }
    }
}
