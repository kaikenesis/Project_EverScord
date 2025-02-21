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
            PlayerData.ECharacter curChar;
            PlayerData.EJob curJob;
            if (GameManager.Instance.PlayerData != null)
            {
                curChar = GameManager.Instance.PlayerData.character;
                curJob = GameManager.Instance.PlayerData.job;
            }
            else
            {
                curChar = PlayerData.ECharacter.Ned;
                curJob = PlayerData.EJob.Dealer;
            }

            characterImg.sprite = GameManager.Instance.InGameUIData.CharacterDatas[(int)curChar].PortraitSourceImg;
            jobImg.sprite = GameManager.Instance.InGameUIData.JodDatas[(int)curJob].IconSourceImg;
        }
    }
}
