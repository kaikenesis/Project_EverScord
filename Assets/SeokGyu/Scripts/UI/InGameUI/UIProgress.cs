using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIProgress : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Slider slider;
        [SerializeField] private float duration = 1.0f;

        public void UpdateFillAmount(float value)
        {
            if(slider != null)
                slider.DOValue(value, duration);
            else if(image != null)
                image.DOFillAmount(value, duration);
        }
    }
}
