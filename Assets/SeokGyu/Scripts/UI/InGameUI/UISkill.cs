using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UISkill : MonoBehaviour
    {
        [SerializeField] private Image skillImg;
        [SerializeField] private Image btnImg;
        [SerializeField] private Image frameImg;
        [SerializeField] private Image panelImg;
        [SerializeField] private Image coverImg;
        [SerializeField] private Slot[] slotSources;

        public void Initialize(int type, Sprite skillSourceImg, Sprite btnSourceImg)
        {
            frameImg.gameObject.GetComponent<RectTransform>().sizeDelta = slotSources[type].ImgSize;
            frameImg.color = slotSources[type].FrameColor;
            frameImg.sprite = slotSources[type].FrameSourceImg;
            panelImg.sprite = slotSources[type].CoverSourceImg;
            coverImg.sprite = slotSources[type].CoverSourceImg;

            skillImg.sprite = skillSourceImg;
            btnImg.sprite = btnSourceImg;
        }

        public void ShowCooldown()
        {
            // 1 -> 0 : 1 - curCooldown / maxCooldown
            coverImg.fillAmount = 0;
        }

        [System.Serializable]
        public class Slot
        {
            [SerializeField] private Sprite frameSourceImg;
            [SerializeField] private Sprite coverSourceImg;
            [SerializeField] private Vector2 imgSize;
            [SerializeField] private Color frameColor;

            public Sprite FrameSourceImg
            {
                get { return frameSourceImg; }
                private set { frameSourceImg = value; }
            }

            public Sprite CoverSourceImg
            {
                get { return coverSourceImg; }
                private set { coverSourceImg = value; }
            }

            public Vector2 ImgSize
            {
                get { return imgSize; }
                private set { imgSize = value; }
            }

            public Color FrameColor
            {
                get { return frameColor; }
                private set { frameColor = value; }
            }
        }
    }
}