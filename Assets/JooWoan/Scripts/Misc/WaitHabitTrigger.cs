using UnityEngine;

namespace EverScord
{
    public class WaitHabitTrigger : MonoBehaviour
    {
        [SerializeField] private Animator anim, defeatAnim;
        [SerializeField] private AnimationClip idleClip, victoryStart, defeatStart;
        [SerializeField] private GameObject weapon;
        [Range(0f, 100f), SerializeField] private float triggerChance;

        public void PlayAnimation(bool isVictory)
        {
            if (isVictory)
            {
                defeatAnim.enabled = false;
                anim.enabled = true;
                anim.Play(victoryStart.name);
                InvokeRepeating(nameof(TriggerWaitHabit), 1f, 1f);
                return;
            }

            if (weapon)
                weapon.SetActive(false);
            
            anim.enabled = false;
            defeatAnim.enabled = true;
            defeatAnim.Play(defeatStart.name);
        }

        private void TriggerWaitHabit()
        {
            if (!IsAnimationPlaying(idleClip.name))
                return;
            
            float randomChance = Random.Range(0f, 100f);

            if (randomChance <= triggerChance)
                anim.SetTrigger(ConstStrings.PARAM_WAITHABIT);
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
    }
}

