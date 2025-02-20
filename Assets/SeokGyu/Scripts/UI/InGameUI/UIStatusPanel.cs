using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIStatusPanel : MonoBehaviour
    {
        [SerializeField] private Image characterImg;
        [SerializeField] private Image jobImg;

        private void Start()
        {
            ECharacter curChar;
            EJob curJob;
            if (GameManager.Instance.userData != null)
            {
                curChar = GameManager.Instance.userData.character;
                curJob = GameManager.Instance.userData.job;
            }
            else
            {
                curChar = ECharacter.UNI;
                curJob = EJob.HEALER;
            }

            characterImg.sprite = GameManager.Instance.InGameUIData.CharacterDatas[(int)curChar].PortraitSourceImg;
            jobImg.sprite = GameManager.Instance.InGameUIData.JodDatas[(int)curJob].IconSourceImg;
        }
    }
}
