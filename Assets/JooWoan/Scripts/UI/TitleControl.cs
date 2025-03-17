using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace EverScord
{
    public class TitleControl : MonoBehaviour
    {
        [SerializeField] private UILogin login;
        [SerializeField] private DOTweenAnimation titleFadeOutTween, loginFadeTween, loginMoveTween;
        private bool hasPressedAnything = false;

        void Start()
        {
            login.ShowLoginUI(false);
        }

        void Update()
        {
            if (Input.anyKeyDown && !hasPressedAnything)
            {
                hasPressedAnything = true;
                StartCoroutine(ShowLoginUI());
            }
        }

        private IEnumerator ShowLoginUI()
        {
            titleFadeOutTween.DORewind();
            titleFadeOutTween.DOPlay();

            yield return new WaitForSeconds(0.3f);
            login.ShowLoginUI(true);

            loginFadeTween.DORewind();
            loginFadeTween.DOPlay();

            loginMoveTween.DORewind();
            loginMoveTween.DOPlay();
        }
    }
}