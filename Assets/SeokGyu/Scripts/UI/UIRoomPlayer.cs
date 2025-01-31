using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EverScord
{
    public class UIRoomPlayer : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image partyMasterImg;
        private GameObject partyOption;
        private RectTransform rectPartyOption;

        public static Action<bool, string, string> OnDisplayPartyOption = delegate { };

        public void Initialize(string name, GameObject partyOption)
        {
            //플레이어 이름, 이미지1(캐릭터 초상화), 이미지2(포지션)
            nameText.text = name;
            this.partyOption = partyOption;
            rectPartyOption = partyOption.GetComponent<RectTransform>();

            //if (PhotonNetwork.IsMasterClient)
            //    partyMasterImg.color = new Color(0, 0, 1, 0);
            //else
            //    partyMasterImg.color = new Color(0, 0, 1, 1);
        }

        

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Right:
                    {
                        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1) break;

                        if(Screen.width - eventData.position.x <= rectPartyOption.sizeDelta.x)
                        {
                            rectPartyOption.pivot = new Vector2(1.0f, 1.0f);
                        }
                        else
                        {
                            rectPartyOption.pivot = new Vector2(0.0f, 1.0f);
                        }
                        partyOption.transform.position = eventData.position;
                        OnDisplayPartyOption?.Invoke(PhotonNetwork.IsMasterClient, nameText.text, PhotonNetwork.NickName);
                    }
                    break;
            }
        }
    }
}
