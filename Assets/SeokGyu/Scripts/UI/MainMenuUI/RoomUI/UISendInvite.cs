using System;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UISendInvite : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;

        public static Action<string> OnSendInvite = delegate { };

        public void SendInvite()
        {
            if (string.IsNullOrEmpty(inputField.text)) return;
            OnSendInvite?.Invoke(inputField.text);
            inputField.text = "";
        }
    }
}
