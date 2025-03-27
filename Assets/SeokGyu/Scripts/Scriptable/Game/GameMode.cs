using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Datas/GameMode", fileName = "newGameMode")]
    public class GameMode : ScriptableObject
    {
        [SerializeField] private int maxDealer = 2;
        [SerializeField] private int maxHealer = 1;
        public int maxPlayer { get; private set; }

        public int MaxDealer
        {
            get { return maxDealer; }
        }

        public int MaxHealer
        {
            get { return maxHealer; }
        }

        public void Initialize()
        {
            maxPlayer = maxDealer + maxHealer;
        }
    }
}
