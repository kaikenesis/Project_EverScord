using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIToggleButton : MonoBehaviour
    {
        [SerializeField] protected GameObject[] toggleObject;
        [SerializeField] protected DoTweenUI[] doTweenUIs;
        [SerializeField] protected bool bPlayMoveAnchor;
        [SerializeField] protected bool bPlayFadeInOut;

        private void Awake()
        {
            for (int i = 0; i < doTweenUIs.Length; i++)
            {
                doTweenUIs.Initialize();
            }
        }

        public virtual void ToggleObject()
        {
            for (int i = 0; i < toggleObject.Length; i++)
            {
                toggleObject[i].SetActive(!toggleObject[i].activeSelf);
            }
            SoundManager.Instance.PlaySound("ButtonSound");
        }

        public void MoveAnchorPos(GameObject rectTransform, Vector2 endPos, float duration)
        {
            
        }

        public void FadeInOut(Image[] images, float endValue, float duration)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].DOFade(endValue, duration);
            }
        }

        //private IEnumerator PlayDoTween()
        //{
        //    yield return new WaitForSeconds(duration);
        //}

        [System.Serializable]
        public class DoTweenUI
        {
            [SerializeField] private RectTransform rectTransform;
            [SerializeField] private Vector2 movePos;
            [SerializeField] private Image image;
            [SerializeField] private float startAlphaValue;
            [SerializeField] private float endAlphaValue;
            [SerializeField] private float duration;
            private Vector2 startPos;

            public void Initialize()
            {
                startPos = rectTransform.anchoredPosition;
            }

            public void FadeInOut(bool bReverse)
            {
                if(bReverse)
                {
                    image.DOFade(startAlphaValue, duration);
                }
                else
                {
                    image.DOFade(endAlphaValue, duration);
                }
            }

            public void MoveAnchorPos(bool bReverse)
            {
                if(bReverse)
                {
                    rectTransform.DOAnchorPos(startPos, duration, false);
                }
                else
                {
                    Vector2 endPos = startPos + movePos;
                    rectTransform.DOAnchorPos(endPos, duration, false);
                }
            }
        }
    }
}
