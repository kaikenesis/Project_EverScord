using EverScord;
using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

public class PhotonLogin : MonoBehaviour
{
    [SerializeField] private TMP_InputField nickName;
    public static Action<string> OnConnectToPhoton = delegate { };

    private void Awake()
    {
        PlayerPrefs.SetString("USERNAME", "");
    }

    public void LoginPhoton()
    {
        string nickName = PlayerPrefs.GetString("USERNAME");
        if (string.IsNullOrEmpty(nickName)) return;

        OnConnectToPhoton?.Invoke(nickName);
    }

    public void SetUserName(string name)
    {
        GameManager.Instance.userName = nickName.text;
        PlayerPrefs.SetString("USERNAME", GameManager.Instance.userName);
    }
}
