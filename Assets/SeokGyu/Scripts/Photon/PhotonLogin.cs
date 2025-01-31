using System;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class PhotonLogin : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nickName;

        public static Action<string> OnConnectToPhoton = delegate { };

        public void LoginPhoton()
        {
            string name = GameManager.Instance.userName;
            if (string.IsNullOrEmpty(name)) return;

            OnConnectToPhoton?.Invoke(name);
        }

        public void SetUserName()
        {
            GameManager.Instance.userName = nickName.text;
        }
    }
}
