using DG.Tweening;
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
        private int seconds = 0;
        private int minutes = 0;
        private bool bPlayTimer = false;

        public static Action OnRequestStopMatch = delegate { };

        private void Awake()
        {
            PhotonMatchController.OnStartTimer += HandleStartTimer;
            PhotonMatchController.OnStopTimer += HandleStopTimer;
            PhotonMatchController.OnMatchComplete += HandleMatchComplete;

            Initialize();
        }

        private void OnDestroy()
        {
            PhotonMatchController.OnStartTimer -= HandleStartTimer;
            PhotonMatchController.OnStopTimer -= HandleStopTimer;
            PhotonMatchController.OnMatchComplete -= HandleMatchComplete;
        }

        public void Initialize()
        {
            seconds = 0;
            minutes = 0;
            timerText.text = string.Format("경과시간 {0}:{1:D2}", minutes, seconds);

            gameObject.SetActive(false);
        }

        #region Handle Methods
        private void HandleStartTimer()
        {
            gameObject.SetActive(true);
            DOTween.PlayForward("MatchPanel");

            bPlayTimer = true;
            StartCoroutine(UpdateTimer());
        }

        private void HandleStopTimer()
        {
            bPlayTimer = false;
            DOTween.PlayBackwards("MatchPanel");
        }
        private void HandleMatchComplete()
        {
            bPlayTimer = false;

            seconds = 0;
            minutes = 0;
            timerText.text = "매칭 완료";
            cancleButton.interactable = false;
        }

        #endregion // Handle Methods
        public void StopMatch()
        {
            SoundManager.Instance.PlaySound("ButtonSound");
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

                timerText.text = string.Format("경과시간 {0}:{1:D2}", minutes, seconds);

                seconds++;

                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}
