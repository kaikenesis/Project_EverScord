using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Datas/PlayerData", fileName = "newPlayerData")]
    public class PlayerData : ScriptableObject
    {
        public enum ECharacter
        {
            Ned,
            Uni,
            Us
        }

        public enum EJob
        {
            Dealer,
            Healer
        }

        public enum EDifficulty
        {
            Story,
            Normal,
            Hard
        }

        [SerializeField] private int maxMoney = 100000000;

        public ECharacter character = ECharacter.Ned;
        public EJob job = EJob.Dealer;
        public EDifficulty difficulty = EDifficulty.Normal;
        public int money = 0;
        private int defaultMoney = 100;

        public void Initialize()
        {
            character = ECharacter.Ned;
            job = EJob.Dealer;
            difficulty = EDifficulty.Normal;
            money = defaultMoney;
        }

        public void DecreaseMoney(int cost)
        {
            money -= cost;
            if (money < 0) money = 0;
        }

        public void IncreaseMoney(int cost)
        {
            money += cost;
            if (money > maxMoney) money = maxMoney;
        }
    }
}
