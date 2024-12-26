using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabLogin : MonoBehaviour
{
    [SerializeField] private string username;
    [SerializeField] private GameObject LogInUI;

    private void Start()
    {
        if(string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "3FCDA";
        }

        PlayerPrefs.SetString("USERNAME", "");
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

    private void UpdateDisplayName(string displayName)
    {
        Debug.Log($"Updating Playfab account's Display name to: {displayName}");
        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = displayName };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameSuccess, OnFailure);
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
        UpdateDisplayName(username);
    }

    private void OnDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log($"You have updated the displayname of the playfab account!");
        LogInUI.SetActive(false);
        SceneController.LoadScene("MainMenu");
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log($"There was an issue with your request {error.GenerateErrorReport()}");
    }
}
