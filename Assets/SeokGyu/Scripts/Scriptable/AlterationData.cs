using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Datas/PortraitData", fileName = "newPortraitData")]
    public class AlterationData : ScriptableObject
    {
        public List<UIFactorSlot> slots;
    }
}
