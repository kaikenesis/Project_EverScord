using UnityEngine;

public class NMAnimationEvent : MonoBehaviour
{
    public void PlaySound(string soundName)
    {
        SoundManager.Instance.PlaySound(soundName);
    }
}
