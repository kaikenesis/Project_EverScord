using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

namespace EverScord
{
    public class HabitTrigger : MonoBehaviour
    {
        [SerializeField] protected Animator anim;
        [SerializeField] protected AnimationClip idleClip;
        [SerializeField] protected GameObject weapon;
        [SerializeField] protected List<AssetReference> sounds;
        [Range(0f, 100f), SerializeField] protected float triggerChance;

        private void OnEnable()
        {
            InvokeRepeating(nameof(TriggerWaitHabit), 3f, 3f);
        }

        protected void TriggerWaitHabit()
        {
            if (anim == null || idleClip == null)
                return;
            
            if (!IsAnimationPlaying(idleClip.name))
                return;
            
            float randomChance = Random.Range(0f, 100f);

            if (randomChance <= triggerChance)
                anim.SetTrigger(ConstStrings.PARAM_WAITHABIT);
            else
                anim.ResetTrigger(ConstStrings.PARAM_WAITHABIT);
        }

        private bool IsAnimationPlaying(string name)
        {
            return anim.GetCurrentAnimatorStateInfo(0).IsName(name);
        }

        private void ShowWeapon()
        {
            if (weapon)
                weapon.SetActive(true);
        }

        private void HideWeapon()
        {
            if (weapon)
                weapon.SetActive(false);
        }

        private void PlaySound(int index)
        {
            if (index >= sounds.Count)
                return;

            SoundManager.Instance.PlaySound(sounds[index].AssetGUID);
        }
    }
}
