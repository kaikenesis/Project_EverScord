using System;
using UnityEngine;

namespace EverScord
{
    [CreateAssetMenu(menuName = "EverScord/Datas/ArmorData", fileName = "newArmorData")]
    public class ArmorData : ScriptableObject
    {
        [SerializeField] private Armor[] armors;

        public Armor[] Armors
        {
            get { return armors; }
            private set { armors = value; }
        }

        [System.Serializable]
        public class Armor
        {
            public enum EType
            {
                Chest,
                Foot,
                Head
            }

            [SerializeField] private EType armorType;
            [SerializeField] private int curLevel = 1;
            [SerializeField] private Sprite[] sourceImg;

            public EType ArmorType
            {
                get { return armorType; }
                private set { armorType = value; }
            }

            public int CurLevel
            {
                get { return curLevel; }
                set { curLevel = value; }
            }

            public Sprite[] SourceImg
            {
                get { return sourceImg; }
                private set { sourceImg = value; }
            }
        }
        
        public Action OnLevelUpArmor;

        public void SubscribeOnLevelUp(Action action)
        {
            OnLevelUpArmor -= action;
            OnLevelUpArmor += action;
        }

        public void UnSubscribeOnLevelUp(Action action)
        {
            OnLevelUpArmor -= action;
        }

        public void LevelUpArmors()
        {
            foreach (var armor in armors)
            {
                armor.CurLevel++;
            }
            OnLevelUpArmor?.Invoke();
        }

        public void ResetArmorLevel()
        {
            foreach (var armor in armors)
            {
                armor.CurLevel = 1;
            }
        }
    }
}
