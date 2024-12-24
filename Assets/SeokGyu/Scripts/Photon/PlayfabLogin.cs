using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

public class PlayfabLogin : MonoBehaviour
{
    [SerializeField] private string username;

    private void Start()
    {
        if(string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "3FCDA";
        }
    }

    private bool IsValidUsername()
    {
        bool isValid = false;

        if (username.Length >= 3 && username.Length <= 24)
            isValid = true;
        
        return isValid;
    }

    private void LoginWithCustomId()
    {
        Debug.Log($"Login to Playfab as {username}");
        var request = new LoginWithCustomIDRequest { CustomId = username, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginCustomIdSuccess, OnFailure);
    }

    

    public void SetUsername(string name)
    {
        username = name;
        PlayerPrefs.SetString("USERNAME", username);
    }

    public void Login()
    {
        if (!IsValidUsername()) return;

        LoginWithCustomId();
    }

    //Callback Method
    private void OnLoginCustomIdSuccess(LoginResult result)
    {
        Debug.Log($"You have logged into Playfab using custom id {username}");
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log($"There was an issue with your request {error.GenerateErrorReport()}");
    }
}
