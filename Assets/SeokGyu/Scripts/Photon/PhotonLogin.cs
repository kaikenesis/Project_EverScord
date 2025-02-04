using System;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class PhotonLogin : MonoBehaviour
    {
        [SerializeField] private TMP_InputField userNameInput;
        private string userName = "";

        public static Action<string> OnConnectToPhoton = delegate { };

        public void LoginPhoton()
        {
            if (string.IsNullOrEmpty(userName)) return;

            OnConnectToPhoton?.Invoke(userName);
        }

        public void SetUserName()
        {
            userName = userNameInput.text;
        }
    }
}
