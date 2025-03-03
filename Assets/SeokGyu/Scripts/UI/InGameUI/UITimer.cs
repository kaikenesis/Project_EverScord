using System.Collections;
using TMPro;
using UnityEngine;

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
        }

        private void StartTimer()
        {
            bStop = false;
            StartCoroutine(UpdateTimer());
        }

        private void ResetTimer()
        {
            bStop = true;
            StopCoroutine(UpdateTimer());
        }

        private IEnumerator UpdateTimer()
        {
            while(!bStop)
            {
                timerText.text = string.Format("진행시간 <color=#{0}>[{1}:{2:D2}]</color>", colorHex, min, sec);
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
