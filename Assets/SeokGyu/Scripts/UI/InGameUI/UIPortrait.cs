using System;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIPortrait : MonoBehaviour
    {
        [SerializeField] private Image characterImg;
        private int photonViewID;
        bool isDead = false;

        private void Awake()
        {
            if (GameManager.Instance.PlayerData != null)
            {
                Initialize(GameManager.Instance.PlayerData.character, 0);
            }
        }

        public void Initialize(PlayerData.ECharacter character, int photonViewID)
        {
            characterImg.sprite = GameManager.Instance.InGameUIData.CharacterDatas[(int)character].PortraitSourceImg;
            this.photonViewID = photonViewID;
        }

        public void UpdatePortrait(int pvID, bool bDead)
        {
            if (photonViewID != pvID || isDead == bDead)
                return;

            if(bDead)
            {
                isDead = true;
                characterImg.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            }
            else
            {
                isDead = false;
                characterImg.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }
}
