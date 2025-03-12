using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Datas/CostData", fileName = "newCostData")]
    public class CostData : ScriptableObject
    {
        [SerializeField] private SlotCost[] slotCostDatas;
        [SerializeField] private StageReward[] stageRewardDatas;

        public SlotCost[] SlotCostDatas
        {
            get { return slotCostDatas; }
            private set { slotCostDatas = value; }
        }

        public StageReward[] StageRewardDatas
        {
            get { return stageRewardDatas; }
            private set { stageRewardDatas = value; }
        }

        [System.Serializable]
        public class SlotCost
        {
            [SerializeField] private int unlock;
            [SerializeField] private int reroll;

            public int Unlock
            {
                get { return unlock; }
                private set { unlock = value; }
            }

            public int Reroll
            {
                get { return reroll; }
                private set { reroll = value; }
            }
        }

        [System.Serializable]
        public class StageReward
        {
            [SerializeField] private int money;

            public int Money
            {
                get { return money; }
            }
        }
    }
}
