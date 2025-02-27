using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIProgress : MonoBehaviour
    {
        protected enum EType
        {
            None,
            PlayerHealth,
            BossHealth,
            StageProgress
        }

        [SerializeField] private Image image;
        [SerializeField] private Slider slider;
        [SerializeField] private float duration = 1.0f;
        [SerializeField] protected EType type = EType.None;

        public void UpdateFillAmount(float value)
        {
            if(slider != null)
                slider.DOValue(value, duration);
            else if(image != null)
                image.DOFillAmount(value, duration);
        }
    }
}
