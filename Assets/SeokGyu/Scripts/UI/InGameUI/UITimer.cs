using EverScord.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace EverScord
{
    public class UITimer : MonoBehaviour
    {
        [SerializeField] private Color32 timerColor;
        [SerializeField] private TMP_Text timerText;
        private bool bStop = true;
        private int min = 0;
        private int sec = 0;
        private string colorHex;

        private void Awake()
        {
            colorHex = ColorUtility.ToHtmlStringRGBA(timerColor);
            StartTimer();

            LevelControl.OnLevelUpdated += HandleLevelUpdated;
            PortalControl.OnLevelClear += HandleTimerPause;
            GameOverControl.OnGameOver += HandleTimerPause;
        }

        private void OnDestroy()
        {
            LevelControl.OnLevelUpdated -= HandleLevelUpdated;
            PortalControl.OnLevelClear -= HandleTimerPause;
            GameOverControl.OnGameOver -= HandleTimerPause;
        }

        private void HandleTimerPause()
        {
            PauseTimer();
        }

        private void HandleLevelUpdated(int curStageNum, bool bCoverScreen)
        {
            if(bCoverScreen == true)
            {
                ResetTimer();
            }
            else
            {
                StartTimer();
            }
        }

        private void StartTimer()
        {
            bStop = false;
            StartCoroutine(UpdateTimer());
        }

        private void ResetTimer()
        {
            sec = 0;
            min = 0;
            SetTimerText();
        }

        private void PauseTimer()
        {
            bStop = true;
            StopCoroutine(UpdateTimer());
        }

        private void SetTimerText()
        {
            int currentRoundNum = GameManager.CurrentLevelIndex + 1;
            timerText.text = string.Format("{0} ¶ó¿îµå <color=#{1}>[{2}:{3:D2}]</color>", currentRoundNum, colorHex, min, sec);
        }

        private IEnumerator UpdateTimer()
        {
            while(!bStop)
            {
                SetTimerText();
                sec++;
                if (sec > 59)
                {
                    min++;
                    sec = 0;
                }

                yield return new WaitForSeconds(1f);
            }

            yield return null;
        }
    }
}
