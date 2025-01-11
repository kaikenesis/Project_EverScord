using UnityEngine;
using EverScord.Armor;
using EverScord.BehaviorTree;

namespace EverScord
{
    public class TestPlayer : MonoBehaviour, IBlackboard
    {
        public IHelmet helmet { get; private set; }
        public IVest vest { get; private set; }
        public IShoes shoes { get; private set; }

        void Awake()
        {
            helmet = new Helmet(10, 10, 10, 10, 10);
            vest = new Vest(10, 10, 10);
            shoes = new Shoes(10, 10, 10);
        }

        public void SetHelmet(IHelmet newHelmet)
        {
            helmet = newHelmet;
        }

        public void SetVest(IVest newVest)
        {
            vest = newVest;
        }

        public void SetShoes(IShoes newShoes)
        {
            shoes = newShoes;
        }
    }
}