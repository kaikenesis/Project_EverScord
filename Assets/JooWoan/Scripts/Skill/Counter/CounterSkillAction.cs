using System;
using System.Collections;
using UnityEngine;
using EverScord.Character;
using Photon.Pun;

namespace EverScord.Skill
{
    public class CounterSkillAction : MonoBehaviour, ISkillAction
    {
        private const float RAYCAST_LENGTH = 100f;

        private CharacterControl activator;
        private CounterSkill skill;
        private CooldownTimer cooldownTimer;
        private Coroutine skillCoroutine;
        private Hovl_Laser laserControl;
        private PhotonView photonView;
        private Coroutine buffCoroutine;

        private EJob ejob;
        private Camera activatorCam;
        private CharacterControl cachedTarget;
        private int skillIndex;

        private float elapsedSkillTime;
        private float elapsedLaserTime;

        private bool toggleLaser = false;
        private bool isOutlineActivated = false;

        public bool CanAttackWhileSkill => false;
        public bool IsUsingSkill
        {
            get
            {
                return skillCoroutine != null;
            }
        }

        public void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
        {
            this.activator  = activator;
            this.skill      = (CounterSkill)skill;
            this.skillIndex = skillIndex;
            this.ejob       = ejob;

            photonView      = activator.CharacterPhotonView;
            activatorCam    = activator.CameraControl.Cam;

            cooldownTimer = new CooldownTimer(skill.Cooldown);
            StartCoroutine(cooldownTimer.RunTimer());
        }

        public void Activate()
        {
            if (cooldownTimer.IsCooldown || IsUsingSkill)
                return;

            cooldownTimer.ResetElapsedTime();
            elapsedLaserTime = skill.DamageInterval;
            isOutlineActivated = false;

            skillCoroutine = StartCoroutine(ActivateSkill());
        }

        private IEnumerator ActivateSkill()
        {
            GameObject barrier = Instantiate(skill.BarrierPrefab, activator.transform);
            barrier.transform.SetParent(CharacterSkill.SkillRoot);

            for (elapsedSkillTime = 0f; elapsedSkillTime <= skill.Duration; elapsedSkillTime += Time.deltaTime)
            {
                UpdateBarrierPosition(barrier.transform, activator);

                if (ejob == EJob.DEALER)
                    OffensiveAction();
                else
                    SupportAction();

                yield return null;
            }

            barrier.transform.SetParent(activator.transform);

            StopBarrier(barrier);
            StopLaser();
            SetOutline(false);

            skillCoroutine = null;
        }

        private void UpdateBarrierPosition(Transform barrierTransform, CharacterControl targetCharacter)
        {
            if (!barrierTransform || !targetCharacter)
                return;
                
            barrierTransform.position = new Vector3(
                targetCharacter.transform.position.x,
                barrierTransform.position.y,
                targetCharacter.transform.position.z
            );
        }

        private void StopBarrier(GameObject barrier)
        {
            ParticleSystem[] barrierParticles = barrier.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < barrierParticles.Length; i++)
                barrierParticles[i].Stop();
        }

        public void OffensiveAction()
        {
            if (activator.PlayerInputInfo.pressedLeftMouseButton)
            {
                activator.SetMouseButtonDown(false);
                toggleLaser = !toggleLaser;

                Action selectedAction = toggleLaser ? ShootLaser : StopLaser;
                selectedAction();

                if (PhotonNetwork.IsConnected && photonView.IsMine)
                    photonView.RPC(nameof(activator.SyncCounterSkill), RpcTarget.Others, activator.MouseRayHitPos, toggleLaser, skillIndex);
            }

            RotateLaser();
            ApplyDamage();
        }
        
        private void ShootLaser()
        {
            if (laserControl)
                return;

            activator.TrackAim();
            activator.PlayerWeapon.FireBullet();

            laserControl = Instantiate(skill.LaserPrefab, CharacterSkill.SkillRoot).GetComponent<Hovl_Laser>();
        }

        private void RotateLaser()
        {
            if (!laserControl)
                return;

            activator.PlayerWeapon.SetShootingStance(activator, true);

            laserControl.transform.position = activator.PlayerWeapon.GunPoint.position;
            
            Vector3 lookDir = activator.MouseRayHitPos - laserControl.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);

            laserControl.transform.localRotation = Quaternion.Lerp(
                laserControl.transform.localRotation,
                lookRotation,
                skill.LaserLerpRate
            );
        }

        private void ApplyDamage()
        {
            if (!laserControl || !toggleLaser)
                return;
            
            elapsedLaserTime += elapsedSkillTime;

            if (elapsedLaserTime > skill.DamageInterval)
            {
                elapsedLaserTime = 0f;

                Vector3 laserOrigin = activator.PlayerWeapon.GunPoint.position;
                Vector3 laserDir = activator.MouseRayHitPos - laserOrigin;

                if (!Physics.Raycast(laserOrigin, laserDir, out RaycastHit hit, Mathf.Infinity, GameManager.EnemyLayer))
                    return;

                float calculatedDamage = DamageCalculator.GetSkillDamage(activator, skill);

                IEnemy monster = hit.transform.GetComponent<IEnemy>();

                if (activator.CharacterPhotonView.IsMine)
                    GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(calculatedDamage, monster);
            }
        }

        private void StopLaser()
        {
            if (!laserControl)
                return;

            activator.PlayerWeapon.SetShootingStance(activator, false);
            laserControl.DisablePrepare();

            int totalChildCount = laserControl.transform.childCount;

            for (int i = totalChildCount - 1; i >= 0; i--)
                laserControl.transform.GetChild(i).SetParent(CharacterSkill.SkillRoot);
            
            Destroy(laserControl.gameObject, 0.1f);

            laserControl = null;
            toggleLaser = false;
        }

        public void SupportAction()
        {
            if (!activator.CharacterPhotonView.IsMine)
                return;

            if (buffCoroutine != null)
                return;

            Ray ray = activatorCam.ScreenPointToRay(activator.PlayerInputInfo.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, RAYCAST_LENGTH, GameManager.PlayerLayer))
            {
                SetOutline(false);
                return;
            }

            if (!isOutlineActivated)
                SetOutline(true, hit);

            if (activator.PlayerInputInfo.pressedLeftMouseButton)
            {
                CharacterControl character = hit.collider.GetComponent<CharacterControl>();
                buffCoroutine = StartCoroutine(GrantBuff(character));

                if (PhotonNetwork.IsConnected && photonView.IsMine)
                    photonView.RPC(nameof(activator.SyncCounterSupport), RpcTarget.Others, character.CharacterPhotonView.ViewID, skillIndex);
            }
        }

        public IEnumerator GrantBuff(CharacterControl target)
        {
            GameObject barrier = Instantiate(skill.BarrierPrefab);
            barrier.transform.SetParent(CharacterSkill.SkillRoot);

            // Increase target stat
            
            while (skillCoroutine != null)
            {
                UpdateBarrierPosition(barrier.transform, target);
                yield return null;
            }

            barrier.transform.SetParent(target.transform);
            StopBarrier(barrier);

            buffCoroutine = null;
        }

        public void SyncGrantBuff(int viewID)
        {
            CharacterControl target = GameManager.Instance.PlayerDict[viewID];
            buffCoroutine = StartCoroutine(GrantBuff(target));
        }

        private void SetOutline(bool state, RaycastHit hit = default)
        {
            if (state && !isOutlineActivated)
            {
                isOutlineActivated = true;
                cachedTarget = hit.collider.GetComponent<CharacterControl>();
                cachedTarget.SetCharacterOutline(true);
            }
            else if (isOutlineActivated)
            {
                isOutlineActivated = false;
                cachedTarget.SetCharacterOutline(false);
            }
        }
    }
}
