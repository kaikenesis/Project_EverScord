using EverScord.Character;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class UIDisplayParty : MonoBehaviour
    {
        [SerializeField] private GameObject portrait;
        private List<GameObject> portraits = new List<GameObject>();

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

            for (int i = 1; i < list.Count; i++)
            {
                Debug.Log($"{list[i].gameObject} : {list[i].ViewID}");

                if (portraits.Count < list.Count - 1)
                {
                    GameObject obj = Instantiate(portrait, transform);
                    portraits.Add(obj);

                    CharacterControl characterControl = list[i].gameObject.GetComponent<CharacterControl>();
                    Debug.Log($"{characterControl.CharacterType} , {characterControl.CharacterJob}");

                    obj.GetComponent<UIPortrait>().Initialize(characterControl.CharacterType, list[i].ViewID);
                    obj.GetComponentInChildren<UIJobIcon>().Initialize(characterControl.CharacterJob, list[i].ViewID);
                }
            }
        }
    }
}
