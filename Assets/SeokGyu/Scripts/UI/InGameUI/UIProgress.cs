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
        private Color initialFillColor;

        protected virtual void Awake()
        {
            if (image)
                initialFillColor = image.color;
        }

        public void UpdateFillAmount(float value)
        {
            if(slider != null)
                slider.DOValue(value, duration);
            else if(image != null && image.type == Image.Type.Filled)
                image.DOFillAmount(value, duration);
        }

        public void ChangeFillColor(Color? color)
        {
            if (image == null)
                return;

            if (color == null)
            {
                image.color = initialFillColor;
                return;
            }

            image.DOColor((Color)color, duration);
        }
    }
}
