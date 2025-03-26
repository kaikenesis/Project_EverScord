using UnityEngine;

namespace EverScord.Character
{
    public class CharacterAnimationSound : MonoBehaviour
    {
        private CharacterSoundInfo soundInfo;

        public void Init(CharacterSoundInfo soundInfo)
        {
            this.soundInfo = soundInfo;
        }

        private void PlayReloadingSound()
        {
            SoundManager.Instance.PlaySound(soundInfo.Reloading.AssetGUID, 1);
        }
    }
}
