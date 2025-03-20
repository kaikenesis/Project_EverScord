using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMAnimationEvent : MonoBehaviour
{
    public void PlaySound(string soundName)
    {
        SoundManager.Instance.PlaySound(soundName);
    }
}
