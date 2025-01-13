using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public enum ECharacter
    {
        UNI,
        NED,
        US,
        MAX
    }

    public enum EJob
    {
        DEALER,
        HEALER,
        MAX
    }

    public class PlayerData : MonoBehaviour
    {
        public ECharacter character;
        public EJob job;
    }
}
