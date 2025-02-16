using System.Collections;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;

namespace EverScord.Skill
{
    public class GrenadeSkillAction : MonoBehaviour, ISkillAction
    {
        private const float RAY_OVERLAP = 1.2f;

        private CharacterControl activator;
        private GrenadeSkill skill;
        private CooldownTimer cooldownTimer;
        private Transform startPoint, stampedMarker, hitMarker;
        private LineRenderer trajectoryLine;
        private PhotonView photonView;
        private Rigidbody projectile;

        private Coroutine skillCoroutine;
        private Coroutine stampCoroutine;

        private Vector3 throwDir, grenadeImpactPosition;
        private EJob ejob;

        private float force;
        private float estimatedTime = 0f;
        private int skillIndex;

        private bool hasActivated = false;
        public bool IsUsingSkill
        {
            get { return skillCoroutine != null; }
        }

        public bool CanAttackWhileSkill => false;

        public void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
        {
            if (this.activator != null)
                return;

            this.activator = activator;
            this.skill = (GrenadeSkill)skill;
            this.skillIndex = skillIndex;
            this.ejob = ejob;

            cooldownTimer = new CooldownTimer(skill.Cooldown);
            photonView = activator.CharacterPhotonView;

            if (ejob == EJob.DEALER)
                projectile = this.skill.PoisonBomb;

            else if (ejob == EJob.HEALER)
                projectile = this.skill.HealBomb;

            trajectoryLine  = GetComponent<LineRenderer>();

            startPoint      = Instantiate(this.skill.GrenadeStartPoint, activator.transform).transform;
            hitMarker       = Instantiate(this.skill.HitMarker, transform).transform;
            stampedMarker   = Instantiate(this.skill.HitMarker, transform).transform;

            stampedMarker.gameObject.SetActive(false);
            hitMarker.gameObject.SetActive(false);
            trajectoryLine.enabled = false;

            StartCoroutine(cooldownTimer.RunTimer());
        }

        public void Activate()
        {
            if (cooldownTimer.IsCooldown)
                return;

            hasActivated = !hasActivated;

            if (!hasActivated)
            {
                StartCoroutine(ExitSkill());
                return;
            }

            SetHitMarker(true);
            SetLineVisibility(true);
            activator.PlayerUIControl?.SetAimCursor(activator, false);

            skillCoroutine = StartCoroutine(ActivateSkill());
        }

        private IEnumerator ActivateSkill()
        {
            while (hasActivated)
            {
                SetForce();
                Predict();
                
                if (activator.PlayerInputInfo.pressedLeftMouseButton)
                {
                    activator.SetMouseButtonDown(false);
                    Fire();
                }

                if (cooldownTimer.IsCooldown)
                {
                    StartCoroutine(ExitSkill());
                    yield break;
                }

                yield return null;
            }
        }

        private IEnumerator ExitSkill()
        {
            hasActivated = false;

            SetHitMarker(false);
            SetLineVisibility(false);
            activator.PlayerUIControl?.SetAimCursor(activator, true);

            yield return new WaitForSeconds(0.2f);

            if (skillCoroutine != null)
                StopCoroutine(skillCoroutine);
            
            skillCoroutine = null;
        }

        private void SetForce()
        {
            if (!startPoint)
                return;
            
            Vector3 startPos = new Vector3(startPoint.position.x, activator.MouseRayHitPos.y, startPoint.position.z);
            force = Vector3.Distance(activator.MouseRayHitPos, startPos);
        }

        private void Fire()
        {
            if (photonView.IsMine)
            {
                throwDir = startPoint.forward;
                
                if (PhotonNetwork.IsConnected)
                    photonView.RPC(nameof(activator.SyncGrenadeSkill), RpcTarget.Others, activator.MouseRayHitPos, throwDir, skillIndex);
            }

            SetForce();

            Rigidbody thrownObject = Instantiate(projectile, startPoint.position, Quaternion.identity);

            thrownObject.GetComponent<GrenadeImpact>().Init(activator, this);
            thrownObject.AddForce(throwDir * force, ForceMode.Impulse);

            if (stampCoroutine != null)
                StopCoroutine(stampCoroutine);

            stampCoroutine = StartCoroutine(StampMarker());
            cooldownTimer.ResetElapsedTime();
            SetHitMarker(false);
        }

        private void Predict()
        {
            if (!photonView.IsMine)
                return;

            Vector3 currentVelocity = startPoint.forward * (force / projectile.mass);
            Vector3 currentPosition = startPoint.position;
            Vector3 nextPosition;

            int maxPoints = skill.MaxPoints;
            float predictInterval = skill.PredictInterval;

            estimatedTime = 0f;
            UpdateLine(maxPoints, 0, currentPosition);

            for (int i = 1; i < maxPoints; i++)
            {
                currentVelocity = CalculateNewVelocity(currentVelocity, projectile.drag, predictInterval);
                nextPosition = currentPosition + currentVelocity * predictInterval;

                float overlap = Vector3.Distance(currentPosition, nextPosition) * RAY_OVERLAP;
                estimatedTime += predictInterval;

                if (Physics.Raycast(currentPosition, currentVelocity.normalized, out RaycastHit hit, overlap, GameManager.GroundLayer))
                {
                    UpdateLine(i, i - 1, hit.point);
                    MoveHitMarker(hit);
                    break;
                }

                currentPosition = nextPosition;
                UpdateLine(maxPoints, i, currentPosition);
            }
        }

        private void UpdateLine(int positionCount, int index, Vector3 position)
        {
            trajectoryLine.positionCount = positionCount;
            trajectoryLine.SetPosition(index, position);
        }

        private void SetLineVisibility(bool state)
        {
            if (!photonView.IsMine)
                return;
            
            trajectoryLine.enabled = state;
        }

        private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float predictInterval)
        {
            velocity += Physics.gravity * predictInterval;
            velocity *= Mathf.Clamp01(1f - drag * predictInterval);
            return velocity;
        }

        private void MoveHitMarker(RaycastHit hit)
        {
            hitMarker.position = hit.point + hit.normal * skill.HitMarkerGroundOffset;
            // hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
        }

        private void SetHitMarker(bool state)
        {
            if (!photonView.IsMine)
                return;
            
            hitMarker.gameObject.SetActive(state);
        }

        private IEnumerator StampMarker()
        {
            if (!photonView.IsMine)
                yield break;

            stampedMarker.position = hitMarker.position;
            stampedMarker.rotation = hitMarker.rotation;
            stampedMarker.gameObject.SetActive(true);

            yield return new WaitForSeconds(estimatedTime);

            stampedMarker.gameObject.SetActive(false);
            stampCoroutine = null;
        }

        public void SyncGrenadeSkill(Vector3 throwDir)
        {
            this.throwDir = throwDir;
        }

        public void SetGrenadeImpactPosition(Vector3 position)
        {
            grenadeImpactPosition = position;
        }

        public void OffensiveAction()
        {
            Collider[] colliders = Physics.OverlapSphere(grenadeImpactPosition, skill.ExplosionRadius, GameManager.EnemyLayer);
            float calculatedDamage = DamageCalculator.GetSkillDamage(activator, skill);

            for (int i = 0; i < colliders.Length; i++)
            {
                IEnemy enemy = colliders[i].GetComponent<IEnemy>();
                GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(calculatedDamage, enemy);
            }
        }

        public void SupportAction()
        {
            Collider[] colliders = Physics.OverlapSphere(grenadeImpactPosition, skill.ExplosionRadius, GameManager.PlayerLayer);
            float calculatedHeal = skill.BaseHeal;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].transform.root == activator.transform)
                    continue;

                CharacterControl player = colliders[i].GetComponent<CharacterControl>();
                player.IncreaseHP(calculatedHeal);
            }
        }
    }
}
