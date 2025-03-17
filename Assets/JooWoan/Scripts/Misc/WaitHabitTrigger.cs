using UnityEngine;

namespace EverScord
{
    public class WaitHabitTrigger : HabitTrigger
    {
        [SerializeField] protected Animator defeatAnim;
        [SerializeField] protected AnimationClip victoryStart, defeatStart;
        
        public void PlayAnimation(bool isVictory)
        {
            if (defeatAnim == null || anim == null)
                return;
            
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
    }
}

