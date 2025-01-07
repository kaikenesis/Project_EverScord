using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public enum ECharacter
    {
        Uni,
        Ned,
        Us,
        MAX
    }

    public enum EJob
    {
        Dealer,
        Healer,
        MAX
    }

    public class PlayerData : MonoBehaviour
    {
        public ECharacter character;
        public EJob job;
    }
}
