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
        public static Action OnLoginError = delegate { };

        public void LoginPhoton()
        {
            if (string.IsNullOrEmpty(userName))
            {
                OnLoginError?.Invoke();
                return;
            }

            OnConnectToPhoton?.Invoke(userName);
            SoundManager.Instance.PlaySound("ButtonSound");
        }

        public void SetUserName()
        {
            userName = userNameInput.text;
        }
    }
}
