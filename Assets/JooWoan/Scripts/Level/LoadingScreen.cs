using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace EverScord
{
    public class LoadingScreen : MonoBehaviour
    {
        [field: SerializeField] public GameObject ImagesHub;
        [field: SerializeField] public Camera Camera;

        void Awake()
        {
            GameManager.Instance.InitControl(this);
            ImagesHub.SetActive(false);
            Camera.enabled = false;
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
    }
}
