using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Armor
{
    public abstract class ArmorAugment
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        protected void Init(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
