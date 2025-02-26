using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIJobIcon : MonoBehaviour
    {
        [SerializeField] private Image jobImg;
        private int photonViewID;

        private void Awake()
        {
            if (GameManager.Instance.PlayerData != null)
            {
                Initialize(GameManager.Instance.PlayerData.job, 0);
            }
        }

        public void Initialize(PlayerData.EJob job, int photonViewID)
        {
            jobImg.sprite = GameManager.Instance.InGameUIData.JodDatas[(int)job].IconSourceImg;
            this.photonViewID = photonViewID;
        }
    }
}
