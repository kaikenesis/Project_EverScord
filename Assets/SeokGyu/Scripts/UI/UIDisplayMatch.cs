using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIDisplayMatch : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private Button cancleButton;
        private int seconds;
        private int minutes;
        private bool bPlayTimer = false;

        public static Action OnRequestStopMatch = delegate { };

        private void Awake()
        {
            PhotonMatchController.OnStartTimer += HandleStartTimer;
            PhotonMatchController.OnStopTimer += HandleStopTimer;
            PhotonMatchController.OnMatchComplete += HandleMatchComplete;

            seconds = 0;
            minutes = 0;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            PhotonMatchController.OnStartTimer -= HandleStartTimer;
            PhotonMatchController.OnStopTimer -= HandleStopTimer;
            PhotonMatchController.OnMatchComplete -= HandleMatchComplete;
        }

        #region Handle Methods
        private void HandleStartTimer()
        {
            gameObject.SetActive(true);

            bPlayTimer = true;
            StartCoroutine(UpdateTimer());
        }

        private void HandleStopTimer()
        {
            bPlayTimer = false;

            seconds = 0;
            minutes = 0;
            timerText.text = $"WaitTime : {minutes}.{seconds}";
            
            gameObject.SetActive(false);
        }
        private void HandleMatchComplete()
        {
            bPlayTimer = false;

            seconds = 0;
            minutes = 0;
            timerText.text = "Match Complete";
            cancleButton.interactable = false;
        }

        #endregion // Handle Methods
        public void StopMatch()
        {
            OnRequestStopMatch?.Invoke();
        }

        private IEnumerator UpdateTimer()
        {
            while(bPlayTimer)
            {
                if (seconds > 59)
                {
                    minutes++;
                    seconds = 0;
                }

                timerText.text = $"WaitTime : {minutes}.{seconds}";

                seconds++;

                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}
