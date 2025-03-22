using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EverScord
{
    public class LobbyInputController : BaseInputController
    {
        private GraphicRaycaster m_Raycaster;
        private PointerEventData m_PointerEventData;
        private EventSystem m_EventSystem;

        [SerializeField] private ToggleObject popupSetting;
        private bool bActivePopupSetting;
        [SerializeField] private GameObject sendInvite;
        private bool bActiveSendInvite;

        public static Action<bool, Vector2> OnClickedPlayerUI = delegate { };

        private void Awake()
        {
            m_Raycaster = GetComponent<GraphicRaycaster>();
            m_EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

            UISendInvite.OnSendInvite += HandleSendInvite;
        }

        private void OnDestroy()
        {
            UISendInvite.OnSendInvite -= HandleSendInvite;
        }

        private void Update()
        {
            OnMouseClick();
            OnKeyInput();
        }

        protected override void OnMouseClick()
        {
            base.OnMouseClick();

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                m_PointerEventData = new PointerEventData(m_EventSystem);
                m_PointerEventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();

                m_Raycaster.Raycast(m_PointerEventData, results);

                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].gameObject.tag == "UIPlayer")
                        return;
                }

                OnClickedPlayerUI?.Invoke(false, m_PointerEventData.position);
            }
        }

        protected override void OnKeyInput()
        {
            base.OnKeyInput();

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                OnKeyDownEscape();
            }
            if(Input.GetKeyDown(KeyCode.Return))
            {
                OnKeyDownReturn();
            }
        }

        private void OnKeyDownEscape()
        {
            if(bActiveSendInvite)
            {
                if (bActivePopupSetting)
                {
                    SoundManager.Instance.PlaySound("ButtonSound");
                    bActivePopupSetting = false;
                    popupSetting.PlayDoTween(true);
                    return;
                }

                SoundManager.Instance.PlaySound("ButtonSound");
                bActiveSendInvite = false;
                sendInvite.GetComponent<UISendInvite>().PlayDoTween(true);
                return;
            }

            if(bActivePopupSetting)
            {
                SoundManager.Instance.PlaySound("ButtonSound");
                bActivePopupSetting = false;
                popupSetting.PlayDoTween(true);
            }
            else
            {
                SoundManager.Instance.PlaySound("ButtonSound");
                bActivePopupSetting = true;
                popupSetting.PlayDoTween(false);
            }
        }

        private void OnKeyDownReturn()
        {
            if(bActiveSendInvite)
            {
                sendInvite.GetComponent<UISendInvite>().SendInvite();
            }
        }

        #region Handle Methods
        private void HandleSendInvite(string inviteName)
        {
            bActiveSendInvite = false;
        }
        #endregion // Handle Methods

        public void SetActivePopupSetting(bool bActive)
        {
            bActivePopupSetting = bActive;
        }

        public void SetActiveSendInvite(bool bActive)
        {
            bActiveSendInvite = bActive;
        }
    }
}
