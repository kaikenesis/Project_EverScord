using System.Collections;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;
using EverScord.Skill;
using EverScord.Effects;

namespace EverScord.Weapons
{
    public delegate void OnShotFired(int count);

    public class Weapon : MonoBehaviour
    {
        private const float ANIM_TRANSITION = 0.25f;

        [SerializeField] private ParticleSystem shotEffect;
        [SerializeField] private BulletInfo bulletInfo;
        [SerializeField] public float bulletSpeed;

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

        private PhotonView photonView;
        private OnShotFired onShotFired;
        private CooldownTimer cooldownTimer;
        private CharacterAnimation animControl;

        private int currentAmmo;
        private bool isReloading = false;
        public bool IsReloading => isReloading;

        public void Init(CharacterControl shooter)
        {
            photonView  = shooter.CharacterPhotonView;
            animControl = shooter.AnimationControl;
            currentAmmo = MaxAmmo;

            LinkAimPoint();

            if (shooter.PlayerUIControl == null)
                return;

            onShotFired -= shooter.PlayerUIControl.SetAmmoText;
            onShotFired += shooter.PlayerUIControl.SetAmmoText;
        }

        void OnEnable()
        {
            if (cooldownTimer == null)
                cooldownTimer = new CooldownTimer(Cooldown);

            isReloading = false;
            CurrentAmmo = MaxAmmo;
            StartCoroutine(cooldownTimer.RunTimer());
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
                AimPoint.transform.position = GunPoint.transform.position + GunPoint.forward * WeaponRange;
                return;
            }
        }

        public void Shoot(CharacterControl shooter)
        {
            if (CanCeaseFire(shooter))
            {
                SetShootingStance(shooter, false);
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

            SetShootingStance(shooter, true);
            FireBullet();
        }

        public void TryReload(CharacterControl shooter)
        {
            if (!shooter.PlayerInputInfo.pressedReloadButton)
                return;

            if (currentAmmo == MaxAmmo)
                return;

            if (isReloading)
                return;

            if (shooter.IsUsingSkill && !shooter.CurrentSkillAction.CanAttackWhileSkill)
                return;

            StartCoroutine(Reload(shooter));
        }

        private IEnumerator Reload(CharacterControl shooter)
        {
            isReloading = true;
            CharacterAnimation animControl = shooter.AnimationControl;

            shooter.RigControl.SetAimWeight(false);
            shooter.AnimationControl.SetUpperMask(true);
            shooter.PlayerUIControl.SetReloadText();

            animControl.SetBool(ConstStrings.PARAM_ISRELOADING, true);
            animControl.Play(animControl.AnimInfo.Reload);

            float reloadTime = shooter.Stats.DecreasedReload(ReloadTime);
            yield return new WaitForSeconds(reloadTime);

            shooter.SetIsAiming(true);
            shooter.RigControl.SetAimWeight(true);
            animControl.SetBool(ConstStrings.PARAM_ISRELOADING, false);
            yield return new WaitForSeconds(animControl.AnimInfo.ShootStance.length * ANIM_TRANSITION);

            SoundManager.Instance.PlaySound(ConstStrings.SFX_RELOAD_PUMP);
            CurrentAmmo = MaxAmmo;
            cooldownTimer.ResetElapsedTime();
            isReloading = false;
        }

        public void SetGunPointDirection(Vector3 facingDir)
        {
            GunPoint.forward = facingDir;
        }

        public void FireBullet()
        {
            SoundManager.Instance.PlaySound(ConstStrings.SFX_SHOOT);
            shotEffect.Emit(1);

            Vector3 gunpointPos   = GunPoint.position;
            Vector3 bulletVector  = GunPoint.forward * bulletSpeed;

            Bullet bullet         = ResourceManager.Instance.GetFromPool(AssetReferenceManager.Bullet_ID) as Bullet;
            SmokeTrail smokeTrail = ResourceManager.Instance.GetFromPool(AssetReferenceManager.BulletSmoke_ID) as SmokeTrail;

            if (!bullet)
                return;
            else
            {
                bullet.Init(gunpointPos, bulletVector, bulletInfo, photonView.ViewID);
                GameManager.Instance.BulletsControl.AddBullet(bullet, BulletOwner.MINE);
            }

            if (smokeTrail)
            {
                smokeTrail.transform.forward = bulletVector;
                smokeTrail.Init(bullet);
            }

            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncFireBullet), RpcTarget.Others, gunpointPos, bulletVector, bullet.ViewID, bullet.BulletID);
        }

        public void SetShootingStance(CharacterControl shooter, bool state, bool isImmediate = false)
        {
            shooter.SetIsAiming(state);
            shooter.RigControl.SetAimWeight(state);
            shooter.AnimationControl.SetUpperMask(state, isImmediate);

            AnimationClip clip = state ? animControl.AnimInfo.Shoot : animControl.AnimInfo.ShootEnd;
            animControl.Play(clip);

            if (!PhotonNetwork.IsConnected)
                return;

            photonView.RPC(nameof(SyncRig), RpcTarget.Others, shooter.CharacterPhotonView.ViewID, state, state, clip.name);
        }

        private bool CanShoot(CharacterControl shooter)
        {
            if (!shooter.IsShooting)
                return false;

            if (cooldownTimer.IsCooldown)
                return false;

            if (isReloading)
                return false;

            if (shooter.IsUsingSkill && !shooter.CurrentSkillAction.CanAttackWhileSkill)
                return false;

            if (shooter.IsStunned)
                return false;

            if (shooter.HasState(CharState.TELEPORTING))
                return false;

            return true;
        }

        private bool CanCeaseFire(CharacterControl shooter)
        {
            if (isReloading)
                return false;

            if (!shooter.IsAiming)
                return false;

            float cooldownOvertime = cooldownTimer.GetElapsedTime() - Cooldown;

            if (cooldownOvertime <= shooter.AnimationControl.ShootStanceDuration)
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

        #region PUN RPC

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////////////////////////////////////////////////

        [PunRPC]
        private void SyncFireBullet(Vector3 gunpointPos, Vector3 bulletVector, int viewID, int bulletID)
        {
            SoundManager.Instance.PlaySound(ConstStrings.SFX_SHOOT);
            shotEffect.Emit(1);

            Bullet bullet         = ResourceManager.Instance.GetFromPool(AssetReferenceManager.Bullet_ID) as Bullet;
            SmokeTrail smokeTrail = ResourceManager.Instance.GetFromPool(AssetReferenceManager.BulletSmoke_ID) as SmokeTrail;

            if (smokeTrail)
            {
                smokeTrail.transform.forward = bulletVector;
                smokeTrail.Init(bullet);
            }

            if (bullet)
            {
                bullet.Init(gunpointPos, bulletVector, bulletInfo, viewID);
                bullet.SetBulletID(bulletID);
                GameManager.Instance.BulletsControl.AddBullet(bullet, BulletOwner.OTHER);
            }
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

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
