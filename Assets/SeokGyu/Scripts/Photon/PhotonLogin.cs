using System;
using TMPro;
using UnityEngine;

public class PhotonLogin : MonoBehaviour
{
    [SerializeField] private TMP_InputField nickName;
    private string username;
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

    public void SetUsername(string name)
    {
        username = nickName.text;
        PlayerPrefs.SetString("USERNAME", username);
    }
}
