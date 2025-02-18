using UnityEngine;

namespace EverScord
{
    public class MB_AnimHandler : MonoBehaviour
    {
        private Animator animator;
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Transform movePos;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void OnDigStart()
        {
            targetTransform.SetPositionAndRotation(movePos.position, movePos.rotation);
            animator.SetBool("bDig", true);
        }

        public void OnDigEnd()
        {
            animator.SetBool("bDig", false);
        }

        public void OnStartPattern2()
        {
            MB_Controller controller = targetTransform.gameObject.GetComponent<MB_Controller>();
            if(controller is IAction action)
            {
                action.DoAction(IAction.EType.Action1);
            }
        }

        public void OnEndPattern2()
        {
            animator.SetBool("bPattern2", false);
        }
    }
}
