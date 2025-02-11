using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;
using EverScord.Skill;
using ExitGames.Client.Photon.StructWrapping;
using Unity.Mathematics;

namespace EverScord.Weapons
{
    public delegate void OnShotFired(int count);

    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject aimPointPrefab;
        [field: SerializeField] public AssetReferenceGameObject BulletAssetReference    { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject SmokeAssetReference     { get; private set; }
        [SerializeField] private ParticleSystem shotEffect;
        [field: SerializeField] public Transform GunPoint                               { get; private set; }
        [field: SerializeField] public Transform WeaponTransform                        { get; private set; }
        [field: SerializeField] public Transform LeftTarget                             { get; private set; }
        [field: SerializeField] public Transform LeftHint                               { get; private set; }
        [field: SerializeField] public LayerMask ShootableLayer                         { get; private set; }
        [field: SerializeField] public float AimSensitivity                             { get; private set; }
        [field: SerializeField] public float Cooldown                                   { get; private set; }
        [field: SerializeField] public float ReloadTime                                 { get; private set; }
        [field: SerializeField] public float WeaponRange                                { get; private set; }
        [field: SerializeField] public int MaxAmmo                                      { get; private set; }
        public Transform AimPoint                                                       { get; private set; }
        public Camera ShooterCam                                                        { get; private set; }

        [SerializeField] private BulletInfo bulletInfo;
        [SerializeField] public float bulletSpeed;

        private PhotonView photonView;
        private OnShotFired onShotFired;
        private CooldownTimer cooldownTimer;

        private const float ANIM_TRANSITION = 0.25f;
        private int currentAmmo;

        private bool isReloading = false;
        public bool IsReloading => isReloading;

        public void Init(CharacterControl shooter)
        {
            photonView = shooter.CharacterPhotonView;
            ShooterCam = shooter.CameraControl.Cam;

            LinkAimPoint();

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

        private void LinkAimPoint()
        {
            GameObject[] aimpoints = GameObject.FindGameObjectsWithTag(ConstStrings.TAG_AIMPOINT);

            for (int i = 0; i < aimpoints.Length; i++)
            {
                AimPointInfo info = aimpoints[i].GetComponent<AimPointInfo>();

                if (info.ActorNumber != photonView.Owner.ActorNumber)
                    continue;
                
                AimPoint = aimpoints[i].transform;
                return;
            }

            AimPoint = Instantiate(aimPointPrefab).transform;
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
                        nameof(SyncRig),
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
                    nameof(SyncRig),
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

            Vector3 gunpointPos = GunPoint.position;
            Vector3 bulletVector = GunPoint.forward * bulletSpeed;

            GameObject pooledBullet = ResourceManager.Instance.GetFromPool(BulletAssetReference.AssetGUID, transform.position, Quaternion.identity);
            GameObject pooledSmoke  = ResourceManager.Instance.GetFromPool(SmokeAssetReference.AssetGUID,  transform.position, Quaternion.identity);

            Bullet bullet = pooledBullet.GetComponent<Bullet>();
            bullet.Init(gunpointPos, bulletVector, bulletInfo, photonView.ViewID);

            SmokeTrail smokeTrail = pooledSmoke.GetComponent<SmokeTrail>();
            smokeTrail.transform.forward = bulletVector;
            smokeTrail.Init(bullet);

            GameManager.Instance.BulletsControl.AddBullet(bullet, BulletOwner.MINE);

            if (!PhotonNetwork.IsConnected)
                return;

            photonView.RPC(
                nameof(SyncFireBullet),
                RpcTarget.Others,
                gunpointPos, bulletVector,
                bullet.ViewID,
                bullet.BulletID
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
        private void SyncFireBullet(Vector3 gunpointPos, Vector3 bulletVector, int viewID, int bulletID)
        {
            shotEffect.Emit(1);

            GameObject pooledBullet = ResourceManager.Instance.GetFromPool(BulletAssetReference.AssetGUID, transform.position, Quaternion.identity);
            GameObject pooledSmoke  = ResourceManager.Instance.GetFromPool(SmokeAssetReference.AssetGUID,  transform.position, Quaternion.identity);

            Bullet bullet = pooledBullet.GetComponent<Bullet>();
            bullet.Init(gunpointPos, bulletVector, bulletInfo, viewID);
            bullet.SetBulletID(bulletID);

            SmokeTrail smokeTrail = pooledSmoke.GetComponent<SmokeTrail>();
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
