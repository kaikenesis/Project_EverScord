using UnityEngine;

namespace EverScord.GameCamera
{
    public class CharacterCamera : MonoBehaviour
    {
        [field: SerializeField] public Camera Cam { get; private set; }
        [SerializeField] private float smoothTime;

        private Transform target;
        private Vector3 velocity = Vector3.zero;
        private Vector3 nextPos;
        private float initialHeight;

        public void Init(Transform target)
        {
            this.target = target;
            initialHeight = transform.position.y;
        }

        void LateUpdate()
        {
            if (target == null)
                return;

            FollowTarget();
        }

        private void FollowTarget()
        {
            nextPos = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
            nextPos.y = initialHeight;

            transform.position = nextPos;
        }
    }
}
