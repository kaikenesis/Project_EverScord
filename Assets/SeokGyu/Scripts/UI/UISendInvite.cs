using System;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UISendInvite : MonoBehaviour
    {
        [SerializeField] private string displayName;

        public static Action<string> OnSendInvite = delegate { };

        public void SetSendInviteName(TMP_InputField inputField)
        {
            displayName = inputField.text;
        }

        public void SendInvite()
        {
            if (string.IsNullOrEmpty(displayName)) return;
            OnSendInvite?.Invoke(displayName);
        }
    }
}
