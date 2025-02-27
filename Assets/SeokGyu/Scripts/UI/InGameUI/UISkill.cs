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

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q) && typeNum == 0)
            {
                ShowCooldown(3f);
            }
            if(Input.GetKeyDown(KeyCode.R) && typeNum == 1)
            {
                ShowCooldown(7f);
            }
        }

        private void HandleUsedSkill(int skillIndex, float skillCooldown)
        {
            if(typeNum == skillIndex)
            {
                ShowCooldown(skillCooldown);
            }
        }

        public void ShowCooldown(float duration)
        {
            if (bCooldown == true)
                return;

            coverImg.fillAmount = 1f;
            bCooldown = true;
            StartCoroutine(UpdateCooldown(duration));
        }

        private IEnumerator UpdateCooldown(float duration)
        {
            float time = coverImg.fillAmount * duration;
            while(bCooldown)
            {
                time -= Time.deltaTime;
                coverImg.fillAmount = time / duration;
                if (time <= 0.0f)
                    bCooldown = false;

                Debug.Log(time);
                yield return new WaitForSeconds(Time.deltaTime);
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