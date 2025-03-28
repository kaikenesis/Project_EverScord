using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace EverScord
{
    public class UIMatchPresenter : MonoBehaviour
    {
        [SerializeField] private UIMatchModel model;
        [SerializeField] private UIMatchView view;

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

        private void Initialize()
        {
            view.SetTimerText(string.Format("경과시간 {0}:{1:D2}", model.minutes, model.seconds));
        }

        #region Handle Methods
        private void HandleStartTimer()
        {
            view.OnActivateObjects();
            DOTween.PlayForward("MatchPanel");

            model.bPlayTimer = true;
            StartCoroutine(UpdateTimer());
        }

        private void HandleStopTimer()
        {
            model.bPlayTimer = false;
            DOTween.PlayBackwards("MatchPanel");
        }
        private void HandleMatchComplete()
        {
            model.bPlayTimer = false;

            model.seconds = 0;
            model.minutes = 0;
            view.MatchComplete();
        }
        #endregion // Handle Methods

        public void StopMatch()
        {
            SoundManager.Instance.PlaySound("ButtonSound");
            OnRequestStopMatch?.Invoke();
        }

        private IEnumerator UpdateTimer()
        {
            while (model.bPlayTimer)
            {
                if (model.seconds > 59)
                {
                    model.minutes++;
                    model.seconds = 0;
                }

                view.SetTimerText(string.Format("경과시간 {0}:{1:D2}", model.minutes, model.seconds));
                model.seconds++;

                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}

