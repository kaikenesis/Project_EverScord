using System.Collections;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;
using EverScord.Skill;
using EverScord.Effects;
using System.Collections.Generic;

namespace EverScord.Weapons
{
    public delegate void OnShotFired(int count);

    public class Weapon : MonoBehaviour
    {
        private const float ANIM_TRANSITION = 0.25f;

        [field: SerializeField] public LayerMask ShootableLayer { get; private set; }
        public Transform GunPoint                               { get; private set; }
        public Transform WeaponTransform                        { get; private set; }
        public Transform LeftTarget                             { get; private set; }
        public Transform LeftHint                               { get; private set; }
        public Transform AimPoint                               { get; private set; }

        private ParticleSystem shotEffect;
        private PhotonView photonView;
        private OnShotFired onShotFired;
        private CooldownTimer cooldownTimer;
        private CharacterAnimation animControl;
        private WeaponInfo weaponInfo;
        private CharacterSoundInfo soundInfo;
        private int currentAmmo;

        private bool isReloading = false;
        public bool IsReloading => isReloading;

        public void Init(CharacterControl shooter)
        {
            photonView  = shooter.CharacterPhotonView;
            animControl = shooter.AnimationControl;
            weaponInfo  = shooter.CharacterWeaponInfo;
            soundInfo   = shooter.SoundInfo;
            currentAmmo = weaponInfo.MaxAmmo;

            SetupWeapon();
            LinkAimPoint();

            // Set active false to exclude from blink effect initialization
            shotEffect.gameObject.SetActive(false);

            if (shooter.PlayerUIControl == null)
                return;

            onShotFired -= shooter.PlayerUIControl.SetAmmoText;
            onShotFired += shooter.PlayerUIControl.SetAmmoText;
        }

        void Start()
        {
            shotEffect.gameObject.SetActive(true);
        }

        void OnEnable()
        {
            if (cooldownTimer == null)
                cooldownTimer = new CooldownTimer(weaponInfo.Cooldown);

            isReloading = false;
            CurrentAmmo = weaponInfo.MaxAmmo;
            StartCoroutine(cooldownTimer.RunTimer());
        }

        private Transform GetRightHand()
        {
            List<Transform> childs = new();
            Utilities.GetAllChildren(transform, ref childs);

            for (int i = 0; i < childs.Count; i++)
            {
                if (childs[i].gameObject.tag == ConstStrings.TAG_WEAPON_RIGHTHAND)
                    return childs[i];
            }

            Debug.LogWarning("No right hand found. Assign appropriate right hand tag to character");
            return null;
        }

        private void SetupWeapon()
        {
            Transform rightHand = GetRightHand();
            WeaponTransform = Instantiate(weaponInfo.WeaponPrefab, rightHand).transform;

            List<Transform> childs = new();
            Utilities.GetAllChildren(WeaponTransform, ref childs);

            for (int i = 0; i < childs.Count; i++)
            {
                switch (childs[i].gameObject.tag)
                {
                    case ConstStrings.TAG_WEAPON_MUZZLE:
                        shotEffect = childs[i].GetComponent<ParticleSystem>();
                        break;

                    case ConstStrings.TAG_WEAPON_GUNPOINT:
                        GunPoint = childs[i];
                        break;

                    case ConstStrings.TAG_WEAPON_LEFTTARGET:
                        LeftTarget = childs[i];
                        break;

                    case ConstStrings.TAG_WEAPON_LEFTHINT:
                        LeftHint = childs[i];
                        break;

                    default:
                        break;
                }
            }
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
                AimPoint.transform.position = GunPoint.transform.position + GunPoint.forward * weaponInfo.WeaponRange;
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

            if (currentAmmo == weaponInfo.MaxAmmo)
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
            animControl.Play(shooter.AnimInfo.Reload);

            float reloadTime = shooter.Stats.DecreasedReload(weaponInfo.ReloadTime);
            yield return new WaitForSeconds(reloadTime);

            shooter.SetIsAiming(true);
            shooter.RigControl.SetAimWeight(true);
            animControl.SetBool(ConstStrings.PARAM_ISRELOADING, false);
            yield return new WaitForSeconds(shooter.AnimInfo.ShootStance.length * ANIM_TRANSITION);

            SoundManager.Instance.PlaySound(soundInfo.Reloaded.AssetGUID);
            CurrentAmmo = weaponInfo.MaxAmmo;
            cooldownTimer.ResetElapsedTime();
            isReloading = false;
        }

        public void SetGunPointDirection(Vector3 facingDir)
        {
            GunPoint.forward = facingDir;
        }

        public void FireBullet()
        {
            SoundManager.Instance.PlaySound(soundInfo.Gunshot.AssetGUID);
            shotEffect.Emit(1);

            Vector3 gunpointPos   = GunPoint.position;
            Vector3 bulletVector  = GunPoint.forward * weaponInfo.BulletSpeed;

            Bullet bullet         = ResourceManager.Instance.GetFromPool(AssetReferenceManager.Bullet_ID) as Bullet;
            SmokeTrail smokeTrail = ResourceManager.Instance.GetFromPool(AssetReferenceManager.BulletSmoke_ID) as SmokeTrail;

            if (!bullet)
                return;
            else
            {
                bullet.Init(gunpointPos, bulletVector, weaponInfo, photonView.ViewID);
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

            AnimationClip clip = state ? shooter.AnimInfo.Shoot : shooter.AnimInfo.ShootEnd;
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

            float cooldownOvertime = cooldownTimer.GetElapsedTime() - weaponInfo.Cooldown;

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
                bullet.Init(gunpointPos, bulletVector, weaponInfo, viewID);
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
