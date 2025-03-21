using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIDisplayMessageBox : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;

        [field: SerializeField] public PopUpWindowData.EMsgType type { get; private set; }
        [field: SerializeField] public Button yesButton { get; private set; }

        public void SetMessageText(string titleText, string msgText)
        {
            this.titleText.text = titleText;
            messageText.text = msgText;
        }
    }
}
