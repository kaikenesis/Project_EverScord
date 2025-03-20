using UnityEngine;

namespace EverScord.Character
{
    public class CharacterAnimationSound : MonoBehaviour
    {

        private void PlayReloadingSound()
        {
            SoundManager.Instance.PlaySound(ConstStrings.SFX_RELOADING, 1);
        }
    }
}
