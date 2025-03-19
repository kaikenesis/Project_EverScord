using UnityEngine;

namespace EverScord.Character
{
    public class CharacterAnimationSound : MonoBehaviour
    {

        private void PlayReloadingSound()
        {
            SoundManager.Instance.PlaySound(ConstStrings.SOUND_RELOADING, 1);
        }
    }
}
