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
        private Vector2 startPos = new Vector2(0f, 210f);
        private Vector2 movePos = new Vector2(0f, 0f);
        private RectTransform rectTransform;
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
            rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = startPos;
            gameObject.SetActive(false);
            DOTweenAnimation doTweenAnim = GetComponent<DOTweenAnimation>();
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
            rectTransform.DOAnchorPos(movePos, 1.0f, false);

            bPlayTimer = true;
            StartCoroutine(UpdateTimer());
        }

        private void HandleStopTimer()
        {
            StartCoroutine(MoveAnchorPos());
        }
        private void HandleMatchComplete()
        {
            bPlayTimer = false;

            seconds = 0;
            minutes = 0;
            timerText.text = "��Ī �Ϸ�";
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

                timerText.text = $"����ð� {minutes} : {seconds}";

                seconds++;

                yield return new WaitForSeconds(1.0f);
            }
        }

        private IEnumerator MoveAnchorPos()
        {
            bPlayTimer = false;
            rectTransform.DOAnchorPos(startPos, 1.0f, false);

            yield return new WaitForSeconds(1.0f);

            seconds = 0;
            minutes = 0;
            timerText.text = $"����ð� {minutes} : {seconds}";

            gameObject.SetActive(false);
        }
    }
}
