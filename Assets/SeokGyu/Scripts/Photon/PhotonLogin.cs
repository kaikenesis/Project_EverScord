using EverScord;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;

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

    public void SetUserName(string name)
    {
        GameManager.Instance.userName = nickName.text;
    }
}
