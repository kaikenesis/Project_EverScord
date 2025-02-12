using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Game/CostDatas", fileName = "newCostDatas")]
    public class CostDatas : ScriptableObject
    {
        [SerializeField] private SlotCost[] slotCostDatas;

        public SlotCost[] SlotCostDatas
        {
            get { return slotCostDatas; }
            private set { slotCostDatas = value; }
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
    }
}
