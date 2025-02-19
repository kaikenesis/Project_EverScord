using UnityEngine;

namespace EverScord
{
    public class MB_AnimHandler : MonoBehaviour
    {
        private Animator animator;
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Transform movePos;
        [SerializeField] private Transform targetPos;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void OnDig()
        {
            if (animator.GetBool("bPattern2"))
            {
                targetTransform.SetPositionAndRotation(movePos.position, movePos.rotation);
            }
            else if (animator.GetBool("bPattern3"))
            {
                targetTransform.LookAt(targetPos);
                targetTransform.transform.position = targetPos.position;
            }
            else if (animator.GetBool("bPattern4"))
            {

            }
            else if (animator.GetBool("bPattern5"))
            {

            }
        }

        public void OnStartPattern2()
        {
            MB_Controller controller = targetTransform.gameObject.GetComponent<MB_Controller>();
            if(controller is IAction action)
            {
                action.DoAction(IAction.EType.Action1);
            }
        }

        public void OnEndPattern3()
        {
            animator.SetBool("bPattern3", false);
        }
    }
}
