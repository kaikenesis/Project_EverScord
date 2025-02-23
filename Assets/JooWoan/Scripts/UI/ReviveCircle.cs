using UnityEngine;
using UnityEngine.UI;
using EverScord.GameCamera;

namespace EverScord.UI
{
    public class ReviveCircle : MonoBehaviour
    {
        [SerializeField] private Color increaseColor, decreaseColor;
        [SerializeField] private Image reviveBar, buttonImg;
        [SerializeField] private RectTransform button;
        [SerializeField] private Canvas canvas;
        [SerializeField] private float progress; 
        private Camera cam;
        private Transform circleOwner;
        private Vector3 initialButtonPos;

        public void Init(Transform circleOwner)
        {
            this.circleOwner = circleOwner;
            cam = CharacterCamera.CurrentClientCam;
            canvas.worldCamera = cam;

            initialButtonPos = buttonImg.transform.localPosition;
        }

        void OnDisable()
        {
            buttonImg.transform.localPosition = initialButtonPos;
            reviveBar.color = decreaseColor;
            progress = 0f;
            reviveBar.fillAmount = 0f;
        }

        void Update()
        {
            if (!circleOwner)
                return;
            
            transform.position = new Vector3(circleOwner.transform.position.x, transform.position.y, circleOwner.transform.position.z);
            TrackProgress();
        }

        private void TrackProgress()
        {
            float amount = progress / 100f;
            reviveBar.fillAmount = amount;

            float buttonAngle = amount * 360f;
            button.localEulerAngles = new Vector3(0, 0, -buttonAngle);
        }
    }
}
