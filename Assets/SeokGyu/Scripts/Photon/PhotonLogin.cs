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

        // 닉네임 중복체크 필요
        //for(int i = 0; i< PhotonNetwork; i++)
        //{
        //    if (PhotonNetwork.PlayerList[i].NickName == nickName)
        //    {
        //        Debug.Log("Already have same nickName.");
        //        return;
        //    }
        //}

        OnConnectToPhoton?.Invoke(nickName);
    }

    public void SetUserName(string name)
    {
        GameManager.Instance.userName = nickName.text;
        GameManager.Instance.SetUserName(EJob.DEALER, ELevel.NORMAL);
        //PlayerPrefs.SetString("USERNAME", GameManager.Instance.userName);
    }
}
