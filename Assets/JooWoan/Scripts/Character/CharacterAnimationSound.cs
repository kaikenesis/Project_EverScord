using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EverScord.Character
{
    public class CharacterAnimationSound : MonoBehaviour
    {
        [SerializeField] private AssetReference reloadingSound;

        private void PlayReloadingSound()
        {
            SoundManager.Instance.PlaySound(reloadingSound.AssetGUID, 1);
        }
    }
}
