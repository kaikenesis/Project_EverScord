using EverScord.Character;
using Photon.Pun;
using System.Collections.Generic;
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

            for (int i = 1; i < list.Count; i++)
            {
                GameObject obj = Instantiate(portrait, transform);
                Debug.Log($"{list[i].gameObject} : {list[i].ViewID}");
                CharacterControl characterControl = list[i].gameObject.GetComponent<CharacterControl>();

                Debug.Log($"{characterControl.CharacterType} , {characterControl.CharacterJob}");
                obj.GetComponent<UIPortrait>().Initialize(characterControl.CharacterType);
                obj.GetComponentInChildren<UIJobIcon>().Initialize(characterControl.CharacterJob);
                
            }
        }
    }
}
