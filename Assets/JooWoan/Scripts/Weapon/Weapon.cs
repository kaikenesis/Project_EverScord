using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;
using EverScord.Pool;
using EverScord.Skill;
using Photon.Pun.Demo.Asteroids;

namespace EverScord.Weapons
{
    public delegate void OnShotFired(int count);

    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject aimPointPrefab;
        [SerializeField] private ParticleSystem shotEffect;

        [field: SerializeField] public ParticleSystem HitEffect     { get; private set; }  
        [field: SerializeField] public TracerType WeaponTracerType  { get; private set; }
        [field: SerializeField] public Transform GunPoint           { get; private set; }
        [field: SerializeField] public Transform WeaponTransform    { get; private set; }
        [field: SerializeField] public Transform LeftTarget         { get; private set; }
        [field: SerializeField] public Transform LeftHint           { get; private set; }
        [field: SerializeField] public LayerMask ShootableLayer     { get; private set; }
        [field: SerializeField] public float AimSensitivity         { get; private set; }
        [field: SerializeField] public float MinAimDistance         { get; private set; }
        [field: SerializeField] public float Cooldown               { get; private set; }
        [field: SerializeField] public float ReloadTime             { get; private set; }
        [field: SerializeField] public float WeaponRange            { get; private set; }
        [field: SerializeField] public int MaxAmmo                  { get; private set; }
        [field: SerializeField] public int HitEffectCount           { get; private set; }
        public Transform AimPoint                                   { get; private set; }
        public Camera ShooterCam                                    { get; private set; }

        [SerializeField] public float bulletSpeed;

        private PhotonView photonView;
        private OnShotFired onShotFired;
        private CooldownTimer cooldownTimer;
        private LinkedList<Bullet> bullets = new();

        private const float ANIM_TRANSITION = 0.25f;
        private int currentAmmo;

        private bool isReloading = false;
        public bool IsReloading => isReloading;

        public void Init(CharacterControl shooter)
        {
            photonView = shooter.CharacterPhotonView;
            ShooterCam = shooter.CameraControl.Cam;

            AimPoint = Instantiate(aimPointPrefab).transform;
            currentAmmo = MaxAmmo;

            if (shooter.PlayerUIControl == null)
                return;

            onShotFired -= shooter.PlayerUIControl.SetAmmoText;
            onShotFired += shooter.PlayerUIControl.SetAmmoText;
        }

        void OnEnable()
        {
            if (cooldownTimer == null)
                cooldownTimer = new CooldownTimer(Cooldown);

            StartCoroutine(cooldownTimer.RunTimer());
        }

        void OnDisable()
        {
            cooldownTimer.StopTimer();
        }

        public void Shoot(CharacterControl shooter)
        {
            if (isReloading)
                return;

            if (shooter.IsUsingSkill)
                return;

            float cooldownOvertime = cooldownTimer.GetElapsedTime() - Cooldown;
            CharacterAnimation animControl = shooter.AnimationControl;

            if (shooter.IsAiming && (cooldownOvertime > shooter.AnimationControl.ShootStanceDuration))
            {
                shooter.SetIsAiming(false);
                shooter.RigControl.SetAimWeight(false);
                animControl.Play(animControl.AnimInfo.ShootEnd);

                if (PhotonNetwork.IsConnected)
                {
                    photonView.RPC(
                        "SyncRig",
                        RpcTarget.Others,
                        shooter.CharacterPhotonView.ViewID,
                        false, false, animControl.AnimInfo.ShootEnd.name
                    );
                }

                return;
            }

            if (!CanShoot(shooter))
                return;

            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload(shooter));
                return;
            }

            --CurrentAmmo;
            cooldownTimer.ResetElapsedTime();

            shooter.SetIsAiming(true);
            shooter.RigControl.SetAimWeight(true);
            animControl.Play(animControl.AnimInfo.Shoot);

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC(
                    "SyncRig",
                    RpcTarget.Others,
                    shooter.CharacterPhotonView.ViewID,
                    true, true, animControl.AnimInfo.Shoot.name
                );
            }

            FireBullet();
        }

        public void TryReload(CharacterControl shooter)
        {
            if (currentAmmo == MaxAmmo)
                return;

            if (isReloading)
                return;

            if (shooter.IsUsingSkill)
                return;

            if (!shooter.PlayerInputInfo.pressedReloadButton)
                return;

            StartCoroutine(Reload(shooter));
        }

        private IEnumerator Reload(CharacterControl shooter)
        {
            isReloading = true;
            CharacterAnimation animControl = shooter.AnimationControl;

            shooter.RigControl.SetAimWeight(false);
            shooter.PlayerUIControl.SetReloadText();

            animControl.SetBool(ConstStrings.PARAM_ISRELOADING, true);
            animControl.Play(animControl.AnimInfo.Reload);

            yield return new WaitForSeconds(ReloadTime);

            shooter.SetIsAiming(true);
            shooter.RigControl.SetAimWeight(true);
            animControl.SetBool(ConstStrings.PARAM_ISRELOADING, false);

            yield return new WaitForSeconds(animControl.AnimInfo.ShootStance.length * ANIM_TRANSITION);

            CurrentAmmo = MaxAmmo;
            cooldownTimer.ResetElapsedTime();
            isReloading = false;
        }

        public void SetGunPointDirection(Vector3 facingDir)
        {
            GunPoint.forward = facingDir;
        }

        private void FireBullet()
        {
            shotEffect.Emit(1);

            Bullet bullet = PoolManager.Get(WeaponTracerType);

            Vector3 gunpointPos = GunPoint.position;
            Vector3 bulletVector = GunPoint.forward * bulletSpeed;

            bullet.Init(gunpointPos, bulletVector, photonView.ViewID);

            SmokeTrail smokeTrail = PoolManager.GetSmoke();
            smokeTrail.transform.forward = bulletVector;
            smokeTrail.Init(bullet);

            GameManager.Instance.BulletsControl.AddBullet(bullet, BulletOwner.MINE);

            if (!PhotonNetwork.IsConnected)
                return;

            photonView.RPC(
                "SyncFireBullet",
                RpcTarget.Others,
                gunpointPos, bulletVector,
                (int)WeaponTracerType,
                bullet.ViewID
            );
        }


        private bool CanShoot(CharacterControl shooter)
        {
            if (cooldownTimer.IsCooldown)
                return false;

            if (!shooter.IsShooting)
                return false;

            return true;
        }

        public int CurrentAmmo
        {
            get { return currentAmmo; }
            set
            {
                currentAmmo = value;

                if (onShotFired != null)
                    onShotFired(currentAmmo);
            }
        }

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////

        [PunRPC]
        private void SyncFireBullet(Vector3 gunpointPos, Vector3 bulletVector, int tracerType, int viewId)
        {
            shotEffect.Emit(1);

            Bullet bullet = PoolManager.Get((TracerType)tracerType);
            bullet.Init(gunpointPos, bulletVector, viewId);

            SmokeTrail smokeTrail = PoolManager.GetSmoke();
            smokeTrail.transform.forward = bulletVector;
            smokeTrail.Init(bullet);

            GameManager.Instance.BulletsControl.AddBullet(bullet, BulletOwner.OTHER);
        }

        [PunRPC]
        private void SyncRig(int viewID, bool isAiming, bool setAimWeight, string clipName)
        {
            if (photonView.ViewID != viewID)
                return;

            CharacterControl shooter = GameManager.Instance.PlayerDict[viewID];

            shooter.SetIsAiming(isAiming);
            shooter.RigControl.SetAimWeight(setAimWeight);
            shooter.AnimationControl.Play(clipName);
        }

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////
    }
}
