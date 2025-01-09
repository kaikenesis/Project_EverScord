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
        if (string.IsNullOrEmpty(nickName)) return;
        OnConnectToPhoton?.Invoke(nickName);
    }

    public void SetUserName(string name)
    {
        userName = nickName.text;
        PlayerPrefs.SetString("USERNAME", userName);
    }
}
