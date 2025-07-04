using System.Collections;
using UnityEngine;
using EverScord.Character;
using Photon.Pun;

using AnimationInfo = EverScord.Character.AnimationInfo;
using EverScord.UI;
using EverScord.Effects;

namespace EverScord.Skill
{
    public class JumpAttackAction : SkillAction
    {
        public JumpAttackSkill Skill    { get; private set; }
        public bool CanJump             { get; private set; }

        [SerializeField] private AudioSource electricAudio;
        private CharacterAnimation animControl;
        private AnimationInfo animInfo;
        private WaitForSeconds waitSkill;
        private Coroutine counterCoroutine, jumpCoroutine, attackCoroutine;
        private SkillMarker skillMarker;
        private GameObject stanceEffect, stanceEffectPrefab, landingEffectPrefab, shockwavePrefab, slashPrefab;
        private ParticleSystem[] markerParticles;
        private Vector3 landingPosition;
        private LayerMask targetLayer;

        public override void Init(CharacterControl activator, CharacterSkill skill, PlayerData.EJob ejob, int skillIndex)
        {
            base.Init(activator, skill, ejob, skillIndex);

            Skill       = skill as JumpAttackSkill;
            waitSkill   = new WaitForSeconds(Skill.Duration);
            animControl = activator.AnimationControl;
            animInfo    = animControl.AnimInfo;
        }

        void Start()
        {
            GameObject marker   = ResourceManager.Instance.GetAsset<GameObject>(Skill.Marker.AssetGUID);
            stanceEffectPrefab  = ResourceManager.Instance.GetAsset<GameObject>(Skill.StanceEffect.AssetGUID);
            shockwavePrefab     = ResourceManager.Instance.GetAsset<GameObject>(Skill.ShockwaveEffect.AssetGUID);
            slashPrefab         = ResourceManager.Instance.GetAsset<GameObject>(Skill.SlashEffect.AssetGUID);

            if (ejob == PlayerData.EJob.Dealer)
            {
                targetLayer = GameManager.EnemyLayer;
                landingEffectPrefab = ResourceManager.Instance.GetAsset<GameObject>(Skill.ExplosionEffect.AssetGUID);
            }
            else
            {
                targetLayer = GameManager.PlayerLayer;
                landingEffectPrefab = ResourceManager.Instance.GetAsset<GameObject>(Skill.HealEffect.AssetGUID);
            }

            skillMarker = new SkillMarker(activator, transform, marker);
            SkillMarker.SetMarkerColor(skillMarker.StampedMarker.gameObject, Skill.StampMarkerColor);

            markerParticles = skillMarker.Marker.GetComponentsInChildren<ParticleSystem>();
        }

        public override bool Activate()
        {
            if (cooldownTimer.IsCooldown)
                return false;

            if (IsUsingSkill)
                return false;

            activator.SubscribeOnDecreaseHealth(OnDecreaseHealth);
            activator.SetState(SetCharState.ADD, CharState.INVINCIBLE);
            activator.SetState(SetCharState.ADD, CharState.SKILL_STANCE);

            skillCoroutine = StartCoroutine(ActivateSkill());
            return true;
        }

        private IEnumerator ActivateSkill()
        {
            counterCoroutine = StartCoroutine(CounterStance());
            yield return waitSkill;

            if (attackCoroutine == null)
            {
                SoundManager.Instance.PlaySound(Skill.UltEnd.AssetGUID);
                ExitSkill();
            }
        }

        private IEnumerator CounterStance()
        {
            electricAudio.Play();
            SoundManager.Instance.PlaySound(Skill.StanceSfx.AssetGUID);

            stanceEffect = Instantiate(stanceEffectPrefab, CharacterSkill.SkillRoot);
            stanceEffect.transform.position = activator.PlayerTransform.position;

            activator.RigControl.SetAimWeight(false);
            animControl.SetUpperMask(false, true);

            animControl.CrossFade(new AnimationParam(animInfo.CounterStance.name, 0.1f));
            OutlineControl.SetCharacterOutline(activator, true);

            while (!CanJump)
                yield return null;

            if (photonView.IsMine)
            {
                skillMarker.Set(true);
                EffectControl.SetEffectParticles(markerParticles, true);
            }

            animControl.Play(animInfo.Howl);
            SoundManager.Instance.PlaySound(Skill.ChargeSfx1.AssetGUID);
            SoundManager.Instance.PlaySound(Skill.ChargeSfx2.AssetGUID);
            SoundManager.Instance.PlaySound(Skill.ChargeSfx3.AssetGUID);
            SoundManager.Instance.PlaySound(Skill.ChargeSfx4.AssetGUID);
            Invoke(nameof(ExitHowlAnimation), animInfo.Howl.length);

            var shockwave = Instantiate(shockwavePrefab, CharacterSkill.SkillRoot);
            var slash     = Instantiate(slashPrefab, CharacterSkill.SkillRoot);

            shockwave.transform.position = new Vector3(activator.PlayerTransform.position.x, shockwave.transform.position.y, activator.PlayerTransform.position.z);
            slash.transform.position     = new Vector3(activator.PlayerTransform.position.x, slash.transform.position.y,     activator.PlayerTransform.position.z);

            jumpCoroutine = StartCoroutine(JumpStance());
        }

        private IEnumerator JumpStance()
        {
            while (attackCoroutine == null)
            {
                if (photonView.IsMine)
                {
                    skillMarker.Move(activator.MouseRayHitPos);
                    landingPosition = skillMarker.Marker.position;
                    landingPosition.y = 0f;
                }

                if (CanJump && activator.PlayerInputInfo.pressedLeftMouseButton)
                {
                    base.Activate();

                    SetJumpMode(false);
                    activator.SetMouseButtonDown(false);

                    attackCoroutine = StartCoroutine(JumpAttack());
                    SendRPC();
                    yield break;
                }

                yield return null;
            }
        }

        public IEnumerator JumpAttack()
        {
            electricAudio.Stop();
            activator.SetState(SetCharState.ADD, CharState.RIGID_ANIMATING);

            float stampTime = animInfo.Jump.length + Skill.LandingDuration;

            skillMarker.Set(false);
            skillMarker.Stamp(stampTime);
            EffectControl.SetEffectParticles(stanceEffect, false);

            CancelInvoke(nameof(ExitHowlAnimation));
            animControl.Play(animInfo.Jump.name);

            SoundManager.Instance.PlaySound(Skill.JumpSfx.AssetGUID);
            SoundManager.Instance.PlaySound(Skill.JumpSfx2.AssetGUID);

            yield return new WaitForSeconds(animInfo.Jump.length);

            activator.SetPosition(landingPosition);
            yield return new WaitForSeconds(Skill.LandingDuration);

            CollisionCheck(landingPosition);

            GameObject landingEffect = Instantiate(landingEffectPrefab, CharacterSkill.SkillRoot);
            landingEffect.transform.position = landingPosition;
            SoundManager.Instance.PlaySound(Skill.LandSfx.AssetGUID);
            animControl.Play(animInfo.Land);
            yield return new WaitForSeconds(animInfo.Land.length);

            ExitSkill(true);
            activator.SetState(SetCharState.REMOVE, CharState.RIGID_ANIMATING);
            yield break;
        }

        private void ExitSkill(bool hasAttacked = false)
        {
            CancelInvoke(nameof(ExitHowlAnimation));

            OutlineControl.SetCharacterOutline(activator, false);

            EffectControl.SetEffectParticles(stanceEffect, false);
            EffectControl.SetEffectParticles(markerParticles, false);

            electricAudio.Stop();

            SetJumpMode(false);
            activator.UnsubscribeOnDecreaseHealth(OnDecreaseHealth);

            if (!hasAttacked)
            {
                cooldownTimer.ResetElapsedTime();
                animControl.CrossFade(new AnimationParam(animInfo.Idle.name, 0.3f));
            }
            else
                animControl.Play(animInfo.Idle.name);

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);

            if (jumpCoroutine != null)
                StopCoroutine(jumpCoroutine);

            if (counterCoroutine != null)
                StopCoroutine(counterCoroutine);

            attackCoroutine = null;
            jumpCoroutine = null;
            counterCoroutine = null;
            skillCoroutine = null;

            activator.SetState(SetCharState.REMOVE, CharState.SKILL_STANCE);
            activator.SetState(SetCharState.REMOVE, CharState.INVINCIBLE);
        }

        public override void ExitSkill()
        {
            return;
        }

        private void ExitHowlAnimation()
        {
            animControl.CrossFade(new AnimationParam(animInfo.CounterStance.name, 0.1f));
        }

        private void CollisionCheck(Vector3 landingPosition)
        {
            if (!photonView.IsMine)
                return;

            Collider[] colliders = Physics.OverlapSphere(landingPosition, SkillInfo.skillSizes[0], targetLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (ejob == PlayerData.EJob.Dealer)
                {
                    IEnemy enemy = colliders[i].GetComponent<IEnemy>();
                    float damage = DamageCalculator.GetSkillDamage(activator, SkillInfo.skillDamage, SkillInfo.skillCoefficient, enemy);
                    GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(activator, damage, enemy);

                    SoundManager.Instance.PlaySound(Skill.DealerLandSfx.AssetGUID);

                    if (enemy is BossRPC boss)
                        boss.SetDebuff(activator, EBossDebuff.SLOW, Skill.SlowDuration, Skill.SlowedAmount);
                }
                else
                {
                    CharacterControl player = colliders[i].GetComponent<CharacterControl>();
                    float heal = DamageCalculator.GetSkillDamage(activator, SkillInfo.skillDamage, SkillInfo.skillCoefficient);
                    player.IncreaseHP(activator, heal, true);

                    SoundManager.Instance.PlaySound(Skill.HealerLandSfx.AssetGUID);
                }
            }
        }

        public override void OffensiveAction() {}

        public override void SupportAction() {}

        public void OnDecreaseHealth()
        {
            activator.UnsubscribeOnDecreaseHealth(OnDecreaseHealth);
            SetJumpMode(true);
        }

        public void SetJumpMode(bool state)
        {
            CanJump = state;
        }

        private void SendRPC()
        {
            if (!photonView.IsMine || !PhotonNetwork.IsConnected)
                return;

            photonView.RPC(nameof(activator.SyncJumpAttackSkill), RpcTarget.Others, activator.MouseRayHitPos, landingPosition, skillIndex);
        }

        public void SyncInfo(Vector3 mouseRayHitPos, Vector3 landingPosition)
        {
            CanJump = true;
            skillMarker.Move(mouseRayHitPos);
            this.landingPosition = landingPosition;
        }
    }
}