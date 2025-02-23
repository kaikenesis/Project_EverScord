using UnityEngine;

namespace EverScord.GameCamera
{
    public class CharacterCamera : MonoBehaviour
    {
        public static Camera CurrentClientCam { get; private set; }
        [field: SerializeField] public Camera Cam { get; private set; }
        [SerializeField] private float smoothTime;

        public static Transform Root { get; private set; }
        private Transform target;
        private Vector3 velocity = Vector3.zero;
        private Vector3 nextPos;
        private float initialHeight;


        public void Init(Transform target, bool isMine)
        {
            this.target = target;
            initialHeight = transform.position.y;

            if (isMine)
                CurrentClientCam = Cam;
        }

        void LateUpdate()
        {
            if (target == null)
                return;

            FollowTarget();
        }

        public static void SetCameraRoot()
        {
            if (Root == null)
                Root = GameObject.FindGameObjectWithTag(ConstStrings.TAG_CAMERAROOT).transform;
        }

        private void FollowTarget()
        {
            nextPos = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
            nextPos.y = initialHeight;

            transform.position = nextPos;
        }
    }
}
