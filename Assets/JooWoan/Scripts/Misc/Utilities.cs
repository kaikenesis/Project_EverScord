using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public static class Utilities
    {
        public static void GetAllChildren(Transform parent, ref List<Transform> children)
        {
            foreach (Transform child in parent)
            {
                children.Add(child);
                GetAllChildren(child, ref children);
            }
        }
    }
}