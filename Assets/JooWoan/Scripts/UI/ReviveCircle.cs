using UnityEngine;
using UnityEngine.UI;
using EverScord.GameCamera;
using EverScord.Character;
using DG.Tweening;

namespace EverScord.UI
{
    public class ReviveCircle : MonoBehaviour
    {
        [SerializeField] private Color increaseColor, decreaseColor;
        [SerializeField] private Image reviveBar, buttonImg;
        [SerializeField] private RectTransform button;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Collider reviveCollider;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float increaseSpeed, decreaseSpeed;
        private Camera cam;
        private Transform circleOwner;
        private Vector3 initialButtonPos;
        private int viewID;
        private float progress; 
        private int revivingPeople = 0;

        void Awake()
        {
            initialButtonPos = buttonImg.transform.localPosition;
        }

        public void Init(Transform circleOwner, int viewID)
        {
            this.circleOwner = circleOwner;
            this.viewID = viewID;

            cam = CharacterCamera.CurrentClientCam;
            canvas.worldCamera = cam;
        }

        void OnEnable()
        {
            ResetCircle();

            DOTween.Rewind(ConstStrings.TWEEN_SHOW_REVIVECIRCLE);
            DOTween.Play(ConstStrings.TWEEN_SHOW_REVIVECIRCLE);
        }

        void OnDisable()
        {
            ResetCircle();
        }

        void Update()
        {
            if (!circleOwner)
                return;
            
            FollowPlayer();
            TrackProgress();

            if (revivingPeople > 0)
                IncreaseProgress(increaseSpeed * Time.deltaTime);
            else
                DecreaseProgress(decreaseSpeed * Time.deltaTime);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other))
                return;

            revivingPeople++;
        }

        void OnTriggerExit(Collider other)
        {
            if (!IsPlayer(other))
                return;
            
            revivingPeople--;
        }

        private void FollowPlayer()
        {
            transform.position = new Vector3(circleOwner.transform.position.x, transform.position.y, circleOwner.transform.position.z);
        }

        private void TrackProgress()
        {
            float amount = progress * 0.01f;
            reviveBar.fillAmount = amount;

            float buttonAngle = amount * 360f;
            button.localEulerAngles = new Vector3(0, 0, -buttonAngle);
        }

        public void IncreaseProgress(float amount)
        {
            progress = Mathf.Min(progress + amount, 100f);
            reviveBar.color = increaseColor;
            buttonImg.color = increaseColor;

            if (progress >= 100f)
            {
                reviveCollider.isTrigger = false;

                DOTween.Rewind(ConstStrings.TWEEN_HIDE_REVIVECIRCLE);
                DOTween.Play(ConstStrings.TWEEN_HIDE_REVIVECIRCLE);

                CharacterControl revived = GameManager.Instance.PlayerDict[viewID];
                revived.StartCoroutine(revived.HandleRevival());
            }
        }

        public void DecreaseProgress(float amount)
        {
            progress = Mathf.Max(progress - amount, 0f);
            reviveBar.color = decreaseColor;
            buttonImg.color = decreaseColor;
        }

        private bool IsPlayer(Collider other)
        {
            if (other.transform.root == circleOwner)
                return false;
            
            if (((1 << other.gameObject.layer) & GameManager.PlayerLayer) == 0)
                return false;

            return true;
        }

        private void ResetCircle()
        {
            buttonImg.transform.localPosition = initialButtonPos;
            progress = 0f;
            reviveBar.fillAmount = 0f;
            canvasGroup.alpha = 0f;

            reviveCollider.isTrigger = true;
        }
    }
}
