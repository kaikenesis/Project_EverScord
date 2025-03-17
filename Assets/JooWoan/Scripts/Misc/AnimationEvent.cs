using UnityEngine;

namespace EverScord
{
    public class AnimationEvent : MonoBehaviour
    {
        [SerializeField] private Animator anim;

        private Animator currentAnim;

        void Awake()
        {
            currentAnim = GetComponent<Animator>();
        }

        void OnDisable()
        {
            if (currentAnim)
                currentAnim.enabled = true;
        }

        private void PlayAnimation(string name)
        {
            anim.Play(name, -1, 0f);
        }

        private void SetAnimator(int state)
        {
            if (!currentAnim)
                return;
            
            if (state == 0)
                currentAnim.enabled = false;
            else if (state == 1)
                currentAnim.enabled = true;
        }
    }
}
