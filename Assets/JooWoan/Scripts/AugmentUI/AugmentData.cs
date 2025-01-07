using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Armor
{
    public class AugmentData
    {
        IDictionary<string, ArmorAugment> augmentDict = new Dictionary<string, ArmorAugment>();

        public void Init()
        {
            // Initialize augment dict
        }

        public ArmorAugment GetAugment(string name)
        {
            if (augmentDict.TryGetValue(name, out ArmorAugment augment))
                return augment;
            
            Debug.LogWarning($"Augment load failed : {name}");
            return null;
        }
    }
}
