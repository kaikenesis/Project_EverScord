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
        [SerializeField] private Sprite master;
        [SerializeField] private Sprite guest;
        private Image image;

        public static Action<bool, string, Vector2> OnDisplayPartyOption = delegate { };

        private void Awake()
        {
            UIInputController.OnClickedPlayerUI += HandleClickedPlayerUI;

            image = GetComponent<Image>();
        }

        private void OnDestroy()
        {
            UIInputController.OnClickedPlayerUI -= HandleClickedPlayerUI;
        }

        private void HandleClickedPlayerUI(bool bVisible, Vector2 mousePos)
        {
            OnDisplayPartyOption?.Invoke(bVisible, nameText.text, mousePos);
        }

        public void Initialize(string name, bool bMaster)
        {
            if(bMaster)
            {
                image.sprite = master;
            }
            else
            {
                image.sprite = guest;
            }

            nameText.text = name;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            switch(eventData.button)
            {
                case PointerEventData.InputButton.Right:
                    {
                        OnDisplayPartyOption?.Invoke(true, nameText.text, eventData.position);
                    }
                    break;
            }
        }

        private void OnUpdatedPortrait()
        {
            
        }
    }
}
