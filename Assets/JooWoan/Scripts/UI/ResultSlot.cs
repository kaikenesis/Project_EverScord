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
        private float currentValue, targetValue;

        void OnEnable()
        {
            fadeTween.DORewind();
            moveTween.DORewind();

            SetCurrentValue(0);
            StartCoroutine(StartTextScroll());
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

        public void Init(float targetValue)
        {
            this.targetValue = targetValue;
        }

        private void SetCurrentValue(float value)
        {
            currentValue = value;
            slot.text = $"{currentValue:F0}";
        }
    }
}
