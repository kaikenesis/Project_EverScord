using UnityEngine;

namespace EverScord.GameCamera
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothTime;
        private Vector3 velocity = Vector3.zero;
        private Vector3 nextPos;
        private float initialHeight;

        void Start()
        {
            initialHeight = transform.position.y;
        }

        void LateUpdate()
        {
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
