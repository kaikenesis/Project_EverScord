using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Datas/PopUpWindowData", fileName = "newPopUpWindowData")]
    public class PopUpWindowData : ScriptableObject
    {
        public enum EMsgType
        {
            None,
            UnlockFactor,
            RerollFactor,
            ApplyFactor,
            CannotGameStart,
        }

        [SerializeField] private Message[] messages;

        public Message[] Messages
        {
            get { return messages; }
        }

        [System.Serializable]
        public class Message
        {
            [SerializeField] private string titleText;
            [SerializeField] private string mainMessage;
            [SerializeField] private string subMessage;
            [SerializeField] private string acceptText;
            [SerializeField] private string cancelText;

            public string TitleText
            {
                get { return titleText; }
                private set { titleText = value; }
            }

            public string MainMessage
            {
                get { return mainMessage; }
                private set { mainMessage = value; }
            }

            public string SubMessage
            {
                get { return subMessage; }
                private set { subMessage = value; }
            }

            public string AcceptText
            {
                get { return acceptText; }
                private set { acceptText = value; }
            }

            public string CancelText
            {
                get { return cancelText; }
                private set { cancelText = value; }
            }
        }
    }
}

