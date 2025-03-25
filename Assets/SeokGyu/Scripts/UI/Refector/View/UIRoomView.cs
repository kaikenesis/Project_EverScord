using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIRoomView : MonoBehaviour
    {
        [field: SerializeField] public GameObject roomContainer;
        [field: SerializeField] public Button[] singleOnlyButtons;
        [field: SerializeField] public Button[] masterOnlyButtons;
        [field: SerializeField] public Button[] gameSettingButtons;

        public void DisplayRoom()
        {

        }
    }
}
