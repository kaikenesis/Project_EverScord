using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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

    public enum ELevel
    {
        NORMAL,
        HARD,
        MAX
    }

    public enum EPhotonState
    {
        NONE,
        MATCH,
        MAX
    }

    public class PlayerData : MonoBehaviour
    {
        public ECharacter character = ECharacter.UNI;
        public EJob job = EJob.DEALER;
        public ELevel curLevel = ELevel.NORMAL;
        public EPhotonState curPhotonState = EPhotonState.NONE;
    }
}
