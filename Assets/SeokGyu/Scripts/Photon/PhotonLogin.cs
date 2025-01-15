using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

public class PhotonLogin : MonoBehaviour
{
    [SerializeField] private TMP_InputField nickName;
    private string userName;
    public static Action<string> OnConnectToPhoton = delegate { };

    private void Awake()
    {
        PlayerPrefs.SetString("USERNAME", "");
    }

    public void LoginPhoton()
    {
        string nickName = PlayerPrefs.GetString("USERNAME");
        // 닉네임 중복체크 필요
        if (string.IsNullOrEmpty(nickName)) return;
        OnConnectToPhoton?.Invoke(nickName);
    }

    public void SetUserName(string name)
    {
        userName = nickName.text;
        PlayerPrefs.SetString("USERNAME", userName);
    }
}
