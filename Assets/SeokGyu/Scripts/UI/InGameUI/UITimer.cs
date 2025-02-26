using System.Collections;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UITimer : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        private bool bStop = false;
        private int min = 0;
        private int sec = 0;

        private void Awake()
        {
            bStop = true;
            StartCoroutine(UpdateTimer());
        }

        private void StartTimer()
        {
            StartCoroutine(UpdateTimer());
        }

        private IEnumerator UpdateTimer()
        {
            while(bStop)
            {
                timerText.text = string.Format("진행시간 [{0}:{1:D2}]", min, sec);
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
