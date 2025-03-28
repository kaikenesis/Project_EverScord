using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIMatchView : ToggleObject
    {
        [field: SerializeField] public TMP_Text timerText { get; private set; }
        [field: SerializeField] public Button cancleButton { get; private set; }

        public void SetTimerText(string text)
        {
            timerText.text = text;
        }

        public void MatchComplete()
        {
            SetTimerText("¸ÅÄª ¿Ï·á");
            cancleButton.interactable = false;
        }
    }
}

