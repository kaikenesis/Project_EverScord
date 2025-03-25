using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;
using EverScord.UI;
using EverScord.GameCamera;
using EverScord.Effects;

namespace EverScord.Skill
{
    public class CounterSkillAction : SkillAction
    {
        private const float RAYCAST_LENGTH = 100f;

        [SerializeField] private AudioSource barrierAudio, laserAudio, electricAudio;

        // Imported Asset from Hovl
        private Hovl_Laser laserControl;

        private CounterSkill skill;
        private CharacterControl cachedTarget;
        private Coroutine buffCoroutine;

        private float elapsedSkillTime;
        private float elapsedLaserTime;

        private bool toggleLaser = false;
        private bool isOutlineActivated = false;
        public override bool CanAttackWhileSkill
        {
            get { return ejob == PlayerData.EJob.Healer; }
        }

        public override void Init(CharacterControl activator, CharacterSkill skill, PlayerData.EJob ejob, int skillIndex)
        {
            base.Init(activator, skill, ejob, skillIndex);
            this.skill = (CounterSkill)skill;
        }

        public override bool Activate()
        {
            if (!base.Activate())
                return false;
            
            elapsedLaserTime = skill.DamageInterval;
            isOutlineActivated = false;

            barrierAudio.Play();
            skillCoroutine = StartCoroutine(ActivateSkill());
            StartCoroutine(GrantBuff(activator));

            return true;
        }

        private IEnumerator ActivateSkill()
        {
            for (elapsedSkillTime = 0f; elapsedSkillTime <= skill.Duration; elapsedSkillTime += Time.deltaTime)
            {
                if (ejob == PlayerData.EJob.Dealer)
                    OffensiveAction();
                else
                    SupportAction();

                yield return null;
            }

            ExitSkill();
        }

        private IEnumerator UpdateBarrierPosition(Transform barrierTransform, Transform targetCharacter)
        {
            while (barrierTransform && targetCharacter)
            {
                barrierTransform.position = new Vector3(
                    targetCharacter.transform.position.x,
                    barrierTransform.position.y,
                    targetCharacter.transform.position.z
                );

                yield return null;
            }
        }

        private void StopBarrier(GameObject barrier)
        {
            EffectControl.SetEffectParticles(barrier, false);
        }

        public override void OffensiveAction()
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

            if (photonView.IsMine)
            {
                activator.TrackAim();
                activator.PlayerWeapon.FireBullet();
            }

            laserAudio.Play();
            electricAudio.Play();

            SoundManager.Instance.PlaySound(skill.LaserStartSfx.AssetGUID);
            SoundManager.Instance.PlaySound(skill.LaserStartSfx2.AssetGUID);

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
            
            elapsedLaserTime += Time.deltaTime;

            if (elapsedLaserTime > skill.DamageInterval)
            {
                elapsedLaserTime = 0f;

                Vector3 laserOrigin = activator.PlayerWeapon.GunPoint.position;
                Vector3 laserDir = activator.MouseRayHitPos - laserOrigin;

                if (!Physics.Raycast(laserOrigin, laserDir, out RaycastHit hit, Mathf.Infinity, GameManager.EnemyLayer))
                    return;

                IEnemy monster = hit.transform.GetComponent<IEnemy>();

                if (activator.CharacterPhotonView.IsMine)
                {
                    float laserDamage = DamageCalculator.GetSkillDamage(activator, SkillInfo.skillDotDamage, SkillInfo.skillCoefficient, monster);
                    GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(activator, laserDamage, monster);
                }
            }
        }

        private void StopLaser()
        {
            if (!laserControl)
                return;

            activator.PlayerWeapon.SetShootingStance(activator, false, true);
            laserControl.DisablePrepare();

            int totalChildCount = laserControl.transform.childCount;

            for (int i = totalChildCount - 1; i >= 0; i--)
                laserControl.transform.GetChild(i).SetParent(CharacterSkill.SkillRoot);

            laserAudio.Stop();
            SoundManager.Instance.StopSound(skill.LaserStartSfx.AssetGUID);
            SoundManager.Instance.StopSound(skill.LaserStartSfx2.AssetGUID);

            Destroy(laserControl.gameObject, 0.1f);

            laserControl = null;
            toggleLaser = false;
        }

        public override void SupportAction()
        {
            if (!activator.CharacterPhotonView.IsMine)
                return;

            if (buffCoroutine != null)
                return;

            Ray ray = CharacterCamera.CurrentClientCam.ScreenPointToRay(activator.PlayerInputInfo.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, RAYCAST_LENGTH, GameManager.PlayerLayer))
            {
                SetOutline(false);
                return;
            }

            if (hit.transform.root == activator.PlayerTransform)
                return;

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
            SoundManager.Instance.PlaySound(skill.BuffSfx.AssetGUID);
            target.ApplyBuff(BuffType.BARRIER, skill.Duration);

            GameObject barrier = Instantiate(skill.BarrierPrefab);
            barrier.transform.SetParent(CharacterSkill.SkillRoot);

            StartCoroutine(UpdateBarrierPosition(barrier.transform, target.transform));

            while (skillCoroutine != null)
                yield return null;

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
                OutlineControl.SetCharacterOutline(cachedTarget, true);
            }
            else if (isOutlineActivated)
            {
                isOutlineActivated = false;
                OutlineControl.SetCharacterOutline(cachedTarget, false);
            }
        }

        public override void ExitSkill()
        {
            laserAudio.Stop();
            barrierAudio.Stop();
            electricAudio.Stop();

            StopLaser();
            SetOutline(false);

            skillCoroutine = null;
        }
    }
}
