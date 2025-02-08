using UnityEngine;
using EverScord.Weapons;
using Photon.Pun;
using EverScord.UI;
using EverScord.GameCamera;
using EverScord.Skill;

namespace EverScord.Character
{
    public class CharacterControl : MonoBehaviour, IPunInstantiateMagicCallback
    {
        [Header("Character")]
        [SerializeField] private float speed;
        [SerializeField] private float gravity;
        [SerializeField] private float mass;
        [SerializeField] private float bodyRotateSpeed;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private Vector3 groundCheckOffset;

        [Header("Weapon")]
        [SerializeField] private Weapon weapon;

        [Header("Skill")]
        [SerializeField] private SkillActionInfo firstSkillActionInfo;
        [SerializeField] private SkillActionInfo secondSkillActionInfo;

        [Header("UI")]
        [SerializeField] private PlayerUI uiPrefab;

        [Header("Camera")]
        [SerializeField] private CharacterCamera cameraPrefab;

        [Header("Rig")]
        [SerializeField] private CharacterRigControl rigLayerPrefab;

        public PlayerUI PlayerUIControl                                 { get; private set; }
        public CharacterRigControl RigControl                           { get; private set; }
        public CharacterAnimation AnimationControl                      { get; private set; }
        public CharacterPhysics PhysicsControl                          { get; private set; }
        public CharacterCamera CameraControl                            { get; private set; }
        public Transform PlayerTransform                                { get; private set; }
        public InputInfo PlayerInputInfo                                { get; private set; }

        public Weapon PlayerWeapon => weapon;
        public PhotonView CharacterPhotonView => photonView;

        private PhotonView photonView;
        private CharacterController controller;
        private Transform uiHub, cameraHub;
        private Vector3 movement, lookPosition, lookDir, moveInput, moveDir;

        void Awake()
        {
            photonView       = GetComponent<PhotonView>();

            PlayerTransform  = transform;
            PhysicsControl   = new CharacterPhysics(gravity, mass);

            controller       = GetComponent<CharacterController>();
            AnimationControl = GetComponent<CharacterAnimation>();

            uiHub            = GameObject.FindGameObjectWithTag(ConstStrings.TAG_UIROOT).transform;
            cameraHub        = GameObject.FindGameObjectWithTag(ConstStrings.TAG_CAMERAROOT).transform;
            
            PlayerUIControl  = Instantiate(uiPrefab, uiHub);
            CameraControl    = Instantiate(cameraPrefab, cameraHub);

            // Unity docs: Set skinwidth 10% of the Radius
            controller.skinWidth = controller.radius * 0.1f;

            AnimationControl.Init(photonView);
            weapon.Init(this);

            RigControl = Instantiate(rigLayerPrefab, AnimationControl.Anim.transform);
            RigControl.Init(AnimationControl.Anim.transform, GetComponent<Animator>(), weapon);
            RigControl.SetAimWeight(false);

            if (!photonView.IsMine)
            {
                PlayerUIControl.gameObject.SetActive(false);
                CameraControl.gameObject.SetActive(false);
            }

            PlayerUIControl.Init(this);
            CameraControl.Init(PlayerTransform);
            
            firstSkillActionInfo.Init(this);
            secondSkillActionInfo.Init(this);

        }

        void Start()
        {
            GameManager.Instance.AddPlayerControl(this);
        }

        void Update()
        {
            if (!photonView.IsMine)
                return;
            
            SetInput();
            SetMovingDirection();

            PhysicsControl.ApplyGravity(this);
            Move();

            AnimationControl.AnimateMovement(this, moveDir);

            TrackAim();
            RotateBody();

            weapon.TryReload(this);
            weapon.Shoot(this);

            UseSkills();
        }

        private void SetInput()
        {
            PlayerInputInfo = InputControl.ReceiveInput();
            PlayerInputInfo = InputControl.GetCameraRelativeInput(PlayerInputInfo, CameraControl.Cam);

            moveInput = PlayerInputInfo.cameraRelativeInput;
        }

        private void SetMovingDirection()
        {
            if (moveInput.magnitude > 1f)
                moveInput.Normalize();

            moveDir = PlayerTransform.InverseTransformDirection(moveInput);
        }

        private void Move()
        {
            movement = new Vector3(moveInput.x, 0, moveInput.z);

            Vector3 velocity = movement * speed;
            velocity.y = PhysicsControl.FallSpeed;

            controller.Move(velocity * Time.deltaTime);
        }

        private void TrackAim()
        {
            Ray ray = CameraControl.Cam.ScreenPointToRay(PlayerInputInfo.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
                return;

            Vector3 cam2GunPointDir = weapon.GunPoint.position - ray.origin;

            if (!Physics.Raycast(ray.origin, cam2GunPointDir, out RaycastHit cam2GunPointRayHit, Mathf.Infinity, groundLayer))
                return;

            Vector3 gunPoint2MouseDir = (hit.point - cam2GunPointRayHit.point).normalized;
            weapon.SetGunPointDirection(gunPoint2MouseDir);

            lookPosition = hit.point;
            lookPosition.y = PlayerTransform.position.y;
            lookDir = lookPosition - PlayerTransform.position;

            float distance = lookDir.magnitude;

            lookDir.Normalize();

            if (distance < 0.5f)
                return;

            if (distance < weapon.MinAimDistance)
                lookPosition = PlayerTransform.position + lookDir * weapon.MinAimDistance;

            Vector3 aimPosition = weapon.GunPoint.position + weapon.GunPoint.forward * weapon.WeaponRange;
            weapon.AimPoint.position = aimPosition;

            if (PhotonNetwork.IsConnected)
                photonView.RPC("SyncTrackAim", RpcTarget.Others, aimPosition, photonView.ViewID);
        }

        private void RotateBody()
        {
            float angle = Vector3.Angle(lookDir, PlayerTransform.forward);

            if (angle > 2f)
            {
                AnimationControl.Rotate(!IsMoving);
                Quaternion lookRotation = Quaternion.LookRotation(lookDir);

                PlayerTransform.rotation = Quaternion.RotateTowards(
                    PlayerTransform.rotation,
                    lookRotation,
                    bodyRotateSpeed * Time.deltaTime
                );
                return;
            }

            AnimationControl.Rotate(false);
        }

        private void UseSkills()
        {
            if (weapon.IsReloading)
                return;

            EJob ejob = EJob.DEALER;
            
            if (GameManager.Instance.userData != null)
                ejob = GameManager.Instance.userData.job;

            if (firstSkillActionInfo.Skill && PlayerInputInfo.pressedFirstSkill)
            {
                firstSkillActionInfo.SkillAction.Activate(ejob);

                if (PhotonNetwork.IsConnected)
                    photonView.RPC("SyncUseSkill", RpcTarget.Others, 1, (int)ejob);
            }

            else if (secondSkillActionInfo.Skill && PlayerInputInfo.pressedSecondSkill)
            {
                secondSkillActionInfo.SkillAction.Activate(ejob);

                if (PhotonNetwork.IsConnected)
                    photonView.RPC("SyncUseSkill", RpcTarget.Others, 2, (int)ejob);
            }
        }

        public void SetIsAiming(bool state)
        {
            IsAiming = state;
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            GameManager.Instance.AddPlayerPhotonView(info.photonView);
        }

        public bool IsGrounded
        {
            get
            {
                return Physics.CheckSphere(
                    PlayerTransform.TransformPoint(groundCheckOffset),
                    groundCheckRadius,
                    groundLayer
                );
            }
        }

        public bool IsUsingSkill
        {
            get
            {
                if (firstSkillActionInfo.SkillAction != null && firstSkillActionInfo.SkillAction.IsUsingSkill)
                    return true;

                if (secondSkillActionInfo.SkillAction != null && secondSkillActionInfo.SkillAction.IsUsingSkill)
                    return true;

                return false;
            }
        }
        
        public bool IsAiming { get; private set; }
        public bool IsShooting => PlayerInputInfo.holdLeftMouseButton;
        public bool IsMoving => moveInput.magnitude > 0;


        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////

        [PunRPC]
        private void SyncTrackAim(Vector3 aimPosition, int viewID)
        {
            if (photonView.ViewID != viewID)
                return;

            weapon.AimPoint.position = aimPosition;
        }

        [PunRPC]
        private void SyncUseSkill(int skillIndex, int ejob)
        {
            EJob ejobType = (EJob)ejob;

            switch (skillIndex)
            {
                case 1:
                    firstSkillActionInfo.SkillAction.Activate(ejobType);
                    break;

                case 2:
                    secondSkillActionInfo.SkillAction.Activate(ejobType);
                    break;
            }
        }

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////

        #region GIZMO
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.4f);

            Gizmos.DrawSphere(
                transform.TransformPoint(groundCheckOffset),
                groundCheckRadius
            );
        }
        #endregion
    }
}
