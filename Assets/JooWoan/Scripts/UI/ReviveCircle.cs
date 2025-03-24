using UnityEngine;
using UnityEngine.UI;
using EverScord.GameCamera;
using EverScord.Character;
using DG.Tweening;
using Photon.Pun;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;

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
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float increaseSpeed, decreaseSpeed;

        private List<CharacterControl> rescuers = new();
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

        void Start()
        {
            audioSource.outputAudioMixerGroup = SoundManager.Instance.SfxMixerGroup;
        }

        public void Init(int viewID)
        {
            reviveTarget = GameManager.Instance.PlayerDict[viewID];
            circleOwner = reviveTarget.PlayerTransform;

            cam = CharacterCamera.CurrentClientCam;
            canvas.worldCamera = cam;
        }

        void OnEnable()
        {
            ResetCircle();
            FadeIn();
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
            {
                if (!audioSource.isPlaying)
                    audioSource.Play();

                IncreaseProgress(increaseSpeed * Time.deltaTime);
            }
            else
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();
                
                DecreaseProgress(decreaseSpeed * Time.deltaTime);
            }
            
            TrackProgress();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other, out var player))
                return;

            if (rescuers.Contains(player))
                return;                

            rescuers.Add(player);
            revivingPeople++;
        }

        void OnTriggerStay(Collider other)
        {
            for (int i = rescuers.Count - 1; i >= 0; i--)
            {
                if (rescuers[i].IsDead)
                {
                    rescuers.RemoveAt(i);
                    revivingPeople--;
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (!IsPlayer(other, out var player))
                return;
            
            rescuers.Remove(player);
            revivingPeople--;
        }

        private void FadeIn()
        {
            canvasGroup.DOFade(1f, 1f)
                .SetDelay(0f)
                .SetEase(Ease.OutQuad);
        }

        public void SetLocalHeight(float height)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, height, transform.localPosition.z);
        }

        private void FadeOut()
        {
            canvasGroup.DOFade(0f, 1f)
                .SetDelay(0f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => gameObject.SetActive(false));
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
                    photonView.RPC(nameof(reviveTarget.SyncExitCircle), RpcTarget.Others, photonView.ViewID);
                }
            }
        }

        public void DecreaseProgress(float amount)
        {
            progress = Mathf.Max(progress - amount, 0f);
            reviveBar.color = decreaseColor;
            buttonImg.color = decreaseColor;
        }

        private bool IsPlayer(Collider other, out CharacterControl player)
        {
            player = null;

            if (other.transform.root == circleOwner)
                return false;
            
            if (((1 << other.gameObject.layer) & GameManager.PlayerLayer) == 0)
                return false;

            player = other.GetComponent<CharacterControl>();
            
            if (player == null)
                return false;

            return true;
        }

        private void ExitCircle()
        {
            revivingPeople = 0;
            reviveCollider.isTrigger = false;

            FadeOut();

            if (reviveTarget.IsDead && reviveTarget.gameObject.activeSelf)
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
