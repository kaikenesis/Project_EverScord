using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;
using Photon.Chat.Demo;

public class PhotonChatController : MonoBehaviour, IChatClientListener
{
    [SerializeField] private string nickName;
    private ChatClient chatClient;

    public static Action<string, string> OnRoomInvite = delegate { };

    private void Start()
    {
        chatClient = new ChatClient(this);
        UIFriend.OnInviteFriend += HandleFriendInvite;
    }

    private void OnDestroy()
    {
        UIFriend.OnInviteFriend -= HandleFriendInvite;
    }

    private void Update()
    {
        chatClient.Service();
    }

    public void ConnectToPhotonChat()
    {
        nickName = PlayerPrefs.GetString("USERNAME");

        Debug.Log("Connecting to Photon Chat");
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues(nickName);
        ChatAppSettings chatSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient.ConnectUsingSettings(chatSettings);
    }

    public void HandleFriendInvite(string recipient)
    {
        chatClient.SendPrivateMessage(recipient, PhotonNetwork.CurrentRoom.Name);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnDisconnected()
    {
        Debug.Log("You have disconnected from the Photon Chat");
    }

    public void OnConnected()
    {
        Debug.Log("You have connected to the Photon Chat");
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        // sender : �߽���, message : �޽�������
        // channelName : ���� ä�� ���� [Sender : Recipient]
        // (������ ���� �޽����� �ִٸ� Ȯ���ϴµ�.. ���� �ڵ忡���� �α��ν� ���� ������
        // ������ ������ �װ� ���� Ȯ���� �� (ex. channelName -> kenesis : kenesis)
        // ���� �ٸ� �޽����� üũ (ex. channelName -> kenesis : other)

        if (!string.IsNullOrEmpty(message.ToString()))
        {
            // Channel Name format [Sender : Recipient]
            string[] splitNames = channelName.Split(new char[] { ':' });
            string senderName = splitNames[0];

            if(!sender.Equals(senderName, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"{sender}: {message}");
                OnRoomInvite?.Invoke(sender, message.ToString());
            }
        }
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }
}
