using UnityEngine;
using UnityEngine.UI;
using EverScord.GameCamera;
using EverScord.Character;
using DG.Tweening;
using Photon.Pun;

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

        private Quaternion initialRotation;
        private Camera cam;
        private Transform circleOwner;
        private CharacterControl reviveTarget;
        private float progress; 
        private int revivingPeople = 0;

        void Awake()
        {
            initialRotation = transform.localRotation;
        }

        public void Init(Transform circleOwner, int viewID)
        {
            this.circleOwner = circleOwner;
            reviveTarget = GameManager.Instance.PlayerDict[viewID];

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

            if (revivingPeople > 0)
                IncreaseProgress(increaseSpeed * Time.deltaTime);
            else
                DecreaseProgress(decreaseSpeed * Time.deltaTime);
            
            TrackProgress();
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
                ExitCircle();

                if (PhotonNetwork.IsConnected)
                {
                    PhotonView photonView = reviveTarget.CharacterPhotonView;
                    photonView.RPC(nameof(reviveTarget.SyncExitCircle), RpcTarget.Others);
                }
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

        private void ExitCircle()
        {
            revivingPeople = 0;
            reviveCollider.isTrigger = false;

            DOTween.Rewind(ConstStrings.TWEEN_HIDE_REVIVECIRCLE);
            DOTween.Play(ConstStrings.TWEEN_HIDE_REVIVECIRCLE);

            reviveTarget.StartCoroutine(reviveTarget.HandleRevival());
        }

        private void ResetCircle()
        {
            transform.localRotation = initialRotation;
            button.localRotation = Quaternion.identity;

            progress = 0f;
            reviveBar.fillAmount = 0f;
            canvasGroup.alpha = 0f;
            revivingPeople = 0;

            reviveCollider.isTrigger = true;
        }

        public void SyncExitCircle()
        {
            // If exit circle already happened on this client (before the rpc), don execute logic
            if (revivingPeople == 0)
                return;
            
            ExitCircle();
        }
    }
}
