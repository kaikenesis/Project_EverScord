using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIJobIcon : MonoBehaviour
    {
        [SerializeField] private Image jobImg;

        private void Awake()
        {
            if (GameManager.Instance.PlayerData != null)
            {
                Initialize(GameManager.Instance.PlayerData.job);
            }
        }

        public void Initialize(PlayerData.EJob job)
        {
            jobImg.sprite = GameManager.Instance.InGameUIData.JodDatas[(int)job].IconSourceImg;
        }
    }
}
