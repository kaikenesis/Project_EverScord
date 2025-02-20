using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Game/PopUpWindowData", fileName = "newPopUpWindowData")]
    public class PopUpWindowData : ScriptableObject
    {
        [SerializeField] private Message[] message;

        public void SetText(int typeNum, out string mainMessage, out string subMessage, out string acceptText, out string refuseText)
        {
            mainMessage = message[typeNum].MainMessage;
            subMessage = message[typeNum].SubMessage;
            acceptText = message[typeNum].AcceptText;
            refuseText = message[typeNum].RefuseText;
        }

        [System.Serializable]
        public class Message
        {
            [SerializeField] private string mainMessage;
            [SerializeField] private string subMessage;
            [SerializeField] private string acceptText;
            [SerializeField] private string refuseText;

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

            public string RefuseText
            {
                get { return refuseText; }
                private set { refuseText = value; }
            }
        }
    }
}

