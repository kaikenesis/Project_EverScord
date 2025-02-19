using System.Collections;
using UnityEngine;
using EverScord.Character;
using Photon.Pun;

using AnimationInfo = EverScord.Character.AnimationInfo;

namespace EverScord.Skill
{
    public class JumpAttackAction : SkillAction
    {
        public JumpAttackSkill Skill    { get; private set; }
        public bool CanJump             { get; private set; }

        private CharacterAnimation animControl;
        private AnimationInfo animInfo;
        private WaitForSeconds waitSkill;
        private Coroutine counterCoroutine, jumpCoroutine, attackCoroutine;
        private SkillMarker skillMarker;
        private GameObject stanceEffect, stanceEffectPrefab, landingEffectPrefab;
        private ParticleSystem[] markerParticles;
        private Vector3 landingPosition;
        private LayerMask targetLayer;
        private float calculatedImpact;

        public override void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
        {
            Skill       = skill as JumpAttackSkill;
            waitSkill   = new WaitForSeconds(Skill.Duration);
            animControl = activator.AnimationControl;
            animInfo    = animControl.AnimInfo;

            base.Init(activator, skill, ejob, skillIndex);
        }

        void Start()
        {
            GameObject marker   = ResourceManager.Instance.GetAsset<GameObject>(Skill.Marker.AssetGUID);
            stanceEffectPrefab  = ResourceManager.Instance.GetAsset<GameObject>(Skill.StanceEffect.AssetGUID);
            
            if (ejob == EJob.DEALER)
            {
                targetLayer = GameManager.EnemyLayer;
                calculatedImpact = DamageCalculator.GetSkillDamage(activator, Skill);
                landingEffectPrefab = ResourceManager.Instance.GetAsset<GameObject>(Skill.ExplosionEffect.AssetGUID);
            }
            else
            {
                targetLayer = GameManager.PlayerLayer;

                // Calculate total heal amount
                calculatedImpact = Skill.BaseHeal;
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
                ExitSkill();
        }

        private IEnumerator CounterStance()
        {
            stanceEffect = Instantiate(stanceEffectPrefab, CharacterSkill.SkillRoot);
            stanceEffect.transform.position = activator.transform.position;

            animControl.CrossFade(new AnimationParam(animInfo.CounterStance.name, 0.1f));
            activator.SetCharacterOutline(true);

            while (!CanJump)
                yield return null;

            if (photonView.IsMine)
            {
                skillMarker.Set(true);
                CharacterSkill.SetEffectParticles(markerParticles, true);
            }

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
            activator.SetState(SetCharState.ADD, CharState.RIGID_ANIMATING);
            cooldownTimer.ResetElapsedTime();

            float stampTime = animInfo.Jump.length + Skill.LandingDuration;

            skillMarker.Set(false);
            skillMarker.Stamp(stampTime);
            CharacterSkill.SetEffectParticles(stanceEffect, false);

            animControl.Play(animInfo.Jump.name);
            yield return new WaitForSeconds(animInfo.Jump.length);

            activator.SetPosition(landingPosition);
            yield return new WaitForSeconds(Skill.LandingDuration);

            CollisionCheck(landingPosition);

            GameObject landingEffect = Instantiate(landingEffectPrefab, CharacterSkill.SkillRoot);
            landingEffect.transform.position = landingPosition;

            animControl.Play(animInfo.Land);
            yield return new WaitForSeconds(animInfo.Land.length);

            ExitSkill(true);
            activator.SetState(SetCharState.REMOVE, CharState.RIGID_ANIMATING);
            yield break;
        }

        private void ExitSkill(bool hasAttacked = false)
        {
            activator.SetCharacterOutline(false);
            SetJumpMode(false);

            CharacterSkill.SetEffectParticles(stanceEffect, false);
            CharacterSkill.SetEffectParticles(markerParticles, false);

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

        private void CollisionCheck(Vector3 landingPosition)
        {
            if (!photonView.IsMine)
                return;

            Collider[] colliders = Physics.OverlapSphere(landingPosition, Skill.Radius, targetLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (ejob == EJob.DEALER)
                {
                    IEnemy enemy = colliders[i].GetComponent<IEnemy>();
                    GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(calculatedImpact, enemy);
                }
                else if (colliders[i].transform.root != activator.transform)
                {
                    CharacterControl player = colliders[i].GetComponent<CharacterControl>();
                    player.IncreaseHP(calculatedImpact);
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