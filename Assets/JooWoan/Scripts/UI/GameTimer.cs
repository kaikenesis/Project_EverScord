using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace EverScord.UI
{
    public class GameTimer : MonoBehaviour
    {
        private TextMeshProUGUI timerText;
        private bool isTimerActivated = false;
        private float elapsedTime = 0;
        private float timeLimit = 0;
        private Color32 initialTextColor;

        private UnityEvent onTimerEnd = new UnityEvent();

        void Awake()
        {
            timerText = GetComponent<TextMeshProUGUI>();
            initialTextColor = timerText.color;
        }

        void Update()
        {
            RunTimer();
        }

        void OnDisable()
        {
            StopTimer();
            onTimerEnd.RemoveAllListeners();
        }

        public void SetTimer(float timeLimit, UnityAction timerEndListener)
        {
            onTimerEnd.AddListener(timerEndListener);
            this.timeLimit = timeLimit;
        }

        public void StartTimer()
        {
            isTimerActivated = true;
            elapsedTime = 0;
        }

        public void StopTimer()
        {
            isTimerActivated = false;
            timerText.text = "0";
            timerText.color = initialTextColor;
        }

        private void RunTimer()
        {
            if (!isTimerActivated)
                return;

            elapsedTime += Time.deltaTime;
            timerText.text = (timeLimit - elapsedTime).ToString("F0");

            if (Mathf.Abs(elapsedTime - timeLimit) < 5f)
                timerText.color = new Color32(255, 28, 28, 255);

            if (elapsedTime >= timeLimit)
            {
                onTimerEnd?.Invoke();
                StopTimer();
            }
        }
    }
}

