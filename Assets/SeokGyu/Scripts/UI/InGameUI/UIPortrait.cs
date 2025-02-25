using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIPortrait : MonoBehaviour
    {
        [SerializeField] private Image characterImg;
        private int photonViewNum;

        private void Awake()
        {
            if (GameManager.Instance.PlayerData != null)
            {
                Initialize(GameManager.Instance.PlayerData.character, 0);
            }
        }

        public void Initialize(PlayerData.ECharacter character, int photonViewNum)
        {
            characterImg.sprite = GameManager.Instance.InGameUIData.CharacterDatas[(int)character].PortraitSourceImg;
            this.photonViewNum = photonViewNum;
        }
    }
}
