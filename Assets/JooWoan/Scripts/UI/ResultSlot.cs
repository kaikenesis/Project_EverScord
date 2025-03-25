using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace EverScord.UI
{
    public class ResultSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI slot;
        [SerializeField] private float speed;
        [SerializeField] private DOTweenAnimation fadeTween, moveTween;
        private AudioSource counterAudio;
        private float currentValue, targetValue, minPlaybackInterval;

        void OnEnable()
        {
            fadeTween.DORewind();
            moveTween.DORewind();

            SetCurrentValue(0);
            StartCoroutine(StartTextScroll());
            StartCoroutine(RepeatCounterSound());
        }

        private IEnumerator StartTextScroll()
        {
            currentValue = 0;

            while (!Mathf.Approximately(currentValue, targetValue))
            {
                SetCurrentValue(Mathf.Lerp(currentValue, targetValue, Time.deltaTime * speed));
                yield return null;
            }

            SetCurrentValue(targetValue);
        }

        private IEnumerator RepeatCounterSound()
        {
            while (targetValue - currentValue >= 1f)
            {
                PlayCounterSound();
                yield return null;
            }
        }

        public void Init(float targetValue, AudioSource counterAudio, float minPlaybackInterval)
        {
            this.targetValue = targetValue;
            this.counterAudio = counterAudio;
            this.minPlaybackInterval = minPlaybackInterval;

            counterAudio.outputAudioMixerGroup = SoundManager.Instance.SfxMixerGroup;
        }

        private void SetCurrentValue(float value)
        {
            currentValue = value;
            slot.text = $"{currentValue:F0}";
        }

        public void PlayCounterSound()
        {
            if (counterAudio.isPlaying && counterAudio.time <= minPlaybackInterval)
                return;

            counterAudio.Play();
        }
    }
}
