using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;
using Photon.Chat.Demo;
using static UnityEngine.GraphicsBuffer;

namespace EverScord
{
    public class PhotonChatController : MonoBehaviour, IChatClientListener
    {
        [SerializeField] private string nickName;
        private ChatClient chatClient;

        public static Action<string, string> OnRoomInvite = delegate { };
        public static Action<string> OnRoomFollow = delegate { };
        public static Action<ChatClient> OnChatConnected = delegate { };
        public static Action<PhotonStatus> OnStatusUpdated = delegate { };
        public static Action OnCreateParty = delegate { };

        private void Awake()
        {
            chatClient = new ChatClient(this);
            PhotonLogin.OnConnectToPhoton += HandleConnectToPhotonChat;
            PhotonMatchController.OnFollowRoom += HandleFollowRoom;
            UISendInvite.OnSendInvite += HandleSendInvite;
        }

        private void OnDestroy()
        {
            PhotonLogin.OnConnectToPhoton -= HandleConnectToPhotonChat;
            PhotonMatchController.OnFollowRoom -= HandleFollowRoom;
            UISendInvite.OnSendInvite -= HandleSendInvite;
        }

        private void Update()
        {
            chatClient.Service();
        }

        #region Handle Methods
        private void HandleConnectToPhotonChat(string nickName)
        {
            ConnectToPhotonChat(nickName);
        }

        private void HandleSendInvite(string recipient)
        {
            string message = "";
            if (PhotonNetwork.InRoom)
            {
                message = PhotonNetwork.CurrentRoom.Name;
                message += ":invite";
            }
            chatClient.SendPrivateMessage(recipient, message);
        }

        private void HandleFollowRoom(string recipient, string message)
        {
            string msg = message + ":follow";
            chatClient.SendPrivateMessage(recipient, msg);
        }
        #endregion

        #region Private Methods
        private void InviteMessage(string sender, string message)
        {
            Debug.Log($"{sender}: {message}");
            OnRoomInvite?.Invoke(sender, message);
        }

        private void FollowMessage(string sender, string message)
        {
            OnRoomFollow?.Invoke(message);
        }

        #endregion

        #region Public Methods
        public void ConnectToPhotonChat(string nickName)
        {
            Debug.Log("Connecting to Photon Chat");
            chatClient.AuthValues = new Photon.Chat.AuthenticationValues(nickName);
            ChatAppSettings chatSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
            chatClient.ConnectUsingSettings(chatSettings);
        }
        #endregion

        #region Callback Chat Methods
        public void DebugReturn(DebugLevel level, string message)
        {

        }

        public void OnDisconnected()
        {
            Debug.Log("You have disconnected from the Photon Chat");
            chatClient.SetOnlineStatus(ChatUserStatus.Offline);
        }

        public void OnConnected()
        {
            Debug.Log("You have connected to the Photon Chat");
            OnChatConnected?.Invoke(chatClient);
            chatClient.SetOnlineStatus(ChatUserStatus.Online);
        }

        public void OnChatStateChange(ChatState state)
        {

        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {

        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            // sender : 발신자, message : 메시지내용
            // channelName : 현재 채널 네임 [Sender : Recipient]
            // (누군가 보낸 메시지가 있다면 확인하는듯.. 현재 코드에서는 로그인시 내가 보내는
            // 내용이 있으니 그거 먼저 확인한 후 (ex. channelName -> user1 : user1)
            // 이후 다른 메시지를 체크 (ex. channelName -> user1 : other)

            // Channel Name format [Sender : Recipient]
            Debug.Log($"sender : {sender},\nmessage : {message},\nchannelName : {channelName}");

            string[] splitNames = channelName.Split(new char[] { ':' });
            string senderName = splitNames[0];

            if (!sender.Equals(senderName, StringComparison.OrdinalIgnoreCase))
            {
                string[] splitMsg = message.ToString().Split(':');
                string roomName = splitMsg[0];
                string typeName = splitMsg[1];

                if (typeName == "invite")
                    InviteMessage(sender, roomName);
                else if (typeName == "follow")
                    FollowMessage(sender, roomName);
            }
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            Debug.Log($"Photon Chat OnSubscribed");
            for (int i = 0; i < channels.Length; i++)
            {
                Debug.Log($"{channels[i]}");
            }
        }

        public void OnUnsubscribed(string[] channels)
        {
            Debug.Log($"Photon Chat OnUnsubscribed");
            for (int i = 0; i < channels.Length; i++)
            {
                Debug.Log($"{channels[i]}");
            }
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            Debug.Log($"Photon Chat OnStatusUpdate: {user} changed to {status}: {message}");
            PhotonStatus newStatus = new PhotonStatus(user, status, (string)message);
            Debug.Log($"Status Update for {user} and its now {status}.");
            OnStatusUpdated?.Invoke(newStatus);
        }

        public void OnUserSubscribed(string channel, string user)
        {

        }

        public void OnUserUnsubscribed(string channel, string user)
        {

        }
        #endregion
    }
}