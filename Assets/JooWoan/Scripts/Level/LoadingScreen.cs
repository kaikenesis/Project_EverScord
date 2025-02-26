using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace EverScord
{
    public class LoadingScreen : MonoBehaviour
    {
        private const float SMOOTH_TIME = 0.4f;
        private const int MAX_BANNER_COUNT = 5;

        [field: SerializeField] public Image CoverImage;
        [field: SerializeField] public GameObject ImageHub;
        [field: SerializeField] public Image LoadingBar;

        [SerializeField] private RectTransform filterRect;
        [SerializeField] private GameObject firstBanner, bannerPrefab;
        [SerializeField] private float bannerSpeed;
        [SerializeField] private List<Sprite> bannerTextures;

        private List<RectTransform> bannerList = new();
        private List<Image> bannerImages = new();
        private float accumulated = 0f;
        private float barVelocity;
        private float initialRectWidth;
        private float bannerRectWidth;
        private float finalBannerPosX;
        private int bannerSpriteIndex = 0;

        void Awake()
        {
            GameManager.Instance.InitControl(this);

            initialRectWidth = filterRect.rect.width;
            InitBanners();
            
            ResetState();
        }

        void Update()
        {
            if (!GameManager.IsLoadingLevel)
                return;
            
            UpdateProgress();
            UpdateBanners();
        }

        private void ResetState()
        {
            ImageHub.SetActive(false);

            Color coverColor = CoverImage.color;
            coverColor.a = 0f;
            CoverImage.color = coverColor;

            LoadingBar.fillAmount = 0f;

            filterRect.sizeDelta = new Vector2(initialRectWidth, filterRect.sizeDelta.y);
            filterRect.anchoredPosition = Vector3.zero;
        }

        private void InitBanners()
        {
            RectTransform banner = firstBanner.GetComponent<RectTransform>();
            bannerList.Add(banner);

            Image image = banner.GetComponentInChildren<Image>();
            bannerImages.Add(image);

            bannerRectWidth = banner.rect.width;
            float initialBannerPosX = banner.anchoredPosition.x;

            finalBannerPosX = initialBannerPosX - bannerRectWidth * (MAX_BANNER_COUNT - 1);

            for (int i = 1; i < MAX_BANNER_COUNT; i++)
            {
                banner = Instantiate(bannerPrefab, firstBanner.transform.parent).GetComponent<RectTransform>();
                banner.anchoredPosition = new Vector2(initialBannerPosX - bannerRectWidth * i, banner.anchoredPosition.y);
                bannerList.Add(banner);

                image = banner.GetComponentInChildren<Image>();
                bannerImages.Add(image);
            }

            Shuffle(bannerTextures);

            for (int i = 0; i < MAX_BANNER_COUNT; i++)
                bannerImages[i].sprite = GetNextSprite();
        }

        public void CoverScreen()
        {
            DOTween.Rewind(ConstStrings.TWEEN_COVER_SCREEN);
            DOTween.Play(ConstStrings.TWEEN_COVER_SCREEN);
        }

        public void ShowScreen()
        {   
            DOTween.Rewind(ConstStrings.TWEEN_SHOW_SCREEN);
            DOTween.Play(ConstStrings.TWEEN_SHOW_SCREEN);
        }

        public void SetProgress(float amount)
        {
            accumulated = amount;
        }

        private void UpdateProgress()
        {
            LoadingBar.fillAmount = Mathf.SmoothDamp(LoadingBar.fillAmount, accumulated, ref barVelocity, SMOOTH_TIME);

            float decreasedLength = initialRectWidth * LoadingBar.fillAmount;

            filterRect.sizeDelta = new Vector2(initialRectWidth - decreasedLength, filterRect.sizeDelta.y);

            filterRect.anchoredPosition = new Vector2(
                decreasedLength,
                filterRect.anchoredPosition.y
            );
        }

        private void UpdateBanners()
        {
            for (int i = 0; i < MAX_BANNER_COUNT; i++)
            {
                float posX = bannerList[i].anchoredPosition.x - bannerSpeed * Time.deltaTime;
                bannerList[i].anchoredPosition = new Vector2(posX, bannerList[i].anchoredPosition.y);

                if (bannerList[i].anchoredPosition.x <= finalBannerPosX)
                {
                    int nextIndex = (i + 1) % MAX_BANNER_COUNT;
                    Vector2 nextPos = bannerList[nextIndex].anchoredPosition;
                    
                    nextPos.x += bannerRectWidth;
                    bannerList[i].anchoredPosition = nextPos;
                    bannerImages[i].sprite = GetNextSprite();
                }
            }
        }

        private Sprite GetNextSprite()
        {
            ++bannerSpriteIndex;

            if (bannerSpriteIndex == bannerTextures.Count)
                bannerSpriteIndex = 0;

            return bannerTextures[bannerSpriteIndex];
        }

        public void Shuffle(List<Sprite> list)
        {
            var count = list.Count;
            var last = count - 1;
            
            for (int i = 0; i < last; i++)
            {
                int randomIndex = Random.Range(i, count);

                Sprite temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
	    }
    }
}
