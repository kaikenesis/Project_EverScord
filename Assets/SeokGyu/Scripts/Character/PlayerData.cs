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
        STORY,
        NORMAL,
        HARD,
        MAX
    }

    public enum EPhotonState
    {
        NONE,
        MATCH,
        STOPMATCH,
        FOLLOW,
        MAX
    }

    public class PlayerData
    {
        public ECharacter character = ECharacter.UNI;
        public EJob job = EJob.DEALER;
        public ELevel curLevel = ELevel.NORMAL;
        public EPhotonState curPhotonState = EPhotonState.NONE;
    }
}
