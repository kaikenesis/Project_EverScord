using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIProgress : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private float duration = 1.0f;

        public void UpdateFillAmount(float value)
        {
            image.DOFillAmount(value, duration);
        }
    }
}
