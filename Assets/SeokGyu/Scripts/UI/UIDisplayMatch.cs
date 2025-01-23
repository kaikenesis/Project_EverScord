using System;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIDisplayMatch : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        private int seconds;
        private int minutes;

        public static Action OnRequestStopMatch = delegate { };

        private void Awake()
        {
            seconds = 0;
            minutes = 0;
        }

        private void UpdateTimer()
        {
            if (seconds > 59) minutes++;
            timerText.text = $"WaitTime : {minutes}.{seconds}";
        }

        public void StopMatch()
        {
            OnRequestStopMatch?.Invoke();
        }
    }
}
