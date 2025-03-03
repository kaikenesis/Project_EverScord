using EverScord.Skill;
using System.Collections;
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
        private int typeNum;
        private bool bCooldown;

        private void Awake()
        {
            SkillAction.OnUsedSkill += HandleUsedSkill;
        }

        private void OnDestroy()
        {
            SkillAction.OnUsedSkill -= HandleUsedSkill;
        }

        public void Initialize(int type, Sprite skillSourceImg, Sprite btnSourceImg)
        {
            typeNum = type;
            frameImg.gameObject.GetComponent<RectTransform>().sizeDelta = slotSources[type].ImgSize;
            frameImg.color = slotSources[type].FrameColor;
            frameImg.sprite = slotSources[type].FrameSourceImg;
            panelImg.sprite = slotSources[type].CoverSourceImg;
            coverImg.sprite = slotSources[type].CoverSourceImg;
            coverImg.fillAmount = 0f;

            skillImg.sprite = skillSourceImg;
            btnImg.sprite = btnSourceImg;
        }

        private void HandleUsedSkill(int skillIndex, float value)
        {
            //Debug.Log($"Index : {skillIndex}, Cooldown : {value}");
            
            if (typeNum == skillIndex)
            {
                coverImg.fillAmount = 1 - value;
            }
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