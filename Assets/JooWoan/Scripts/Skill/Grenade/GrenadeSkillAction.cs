using System.Collections;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;

namespace EverScord.Skill
{
    public class GrenadeSkillAction : MonoBehaviour, ISkillAction
    {
        [SerializeField] private GameObject grenadeStartPoint;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Rigidbody projectile;
        [SerializeField] private Transform hitMarker;
        [SerializeField] private float predictInterval, hitMarkerGroundOffset;
        [SerializeField] private int maxPoints;

        private CharacterControl activator;
        private CharacterSkill skill;
        private int skillIndex;
        private CooldownTimer cooldownTimer;
        private Transform startPoint, stampedMarker;
        private LineRenderer trajectoryLine;
        private PhotonView photonView;

        private Coroutine skillCoroutine;
        private Coroutine stampCoroutine;

        private const float RAY_OVERLAP = 1.2f;
        private float force;
        private float estimatedTime = 0f;

        private bool hasActivated = false;
        public bool IsUsingSkill
        {
            get { return skillCoroutine != null; }
        }

        public void Init(CharacterControl activator, CharacterSkill skill, int skillIndex)
        {
            if (this.activator != null)
                return;

            this.activator = activator;
            this.skill = skill;
            this.skillIndex = skillIndex;

            photonView = activator.CharacterPhotonView;

            cooldownTimer = new CooldownTimer(skill.Cooldown);
            StartCoroutine(cooldownTimer.RunTimer());

            trajectoryLine = GetComponent<LineRenderer>();

            stampedMarker = Instantiate(hitMarker, transform);
            stampedMarker.gameObject.SetActive(false);
            
            startPoint = Instantiate(grenadeStartPoint, activator.transform).transform;

            HideHitMarker();
            SetLineVisibility(false);
        }

        public void Activate(EJob ejob)
        {
            if (cooldownTimer.IsCooldown)
                return;

            hasActivated = !hasActivated;

            if (!hasActivated)
            {
                StartCoroutine(ExitSkill());
                return;
            }

            SetLineVisibility(true);
            activator.PlayerUIControl?.SetAimCursor(false);

            skillCoroutine = StartCoroutine(ActivateSkill());
        }

        private IEnumerator ActivateSkill()
        {
            while (hasActivated)
            {
                SetForce();
                Predict();
                
                if (activator.PlayerInputInfo.pressedLeftMouseButton)
                    Fire();

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

            HideHitMarker();
            SetLineVisibility(false);
            activator.PlayerUIControl?.SetAimCursor(true);

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
            Rigidbody thrownObject = Instantiate(projectile, startPoint.position, Quaternion.identity);
            thrownObject.AddForce(startPoint.forward * force, ForceMode.Impulse);

            if (stampCoroutine != null)
                StopCoroutine(stampCoroutine);

            stampCoroutine = StartCoroutine(StampMarker());

            cooldownTimer.ResetElapsedTime();
        }

        private void Predict()
        {
            Vector3 currentVelocity = startPoint.forward * (force / projectile.mass);
            Vector3 currentPosition = startPoint.position;
            Vector3 nextPosition;

            UpdateLine(maxPoints, 0, currentPosition);
            estimatedTime = 0f;

            for (int i = 1; i < maxPoints; i++)
            {
                currentVelocity = CalculateNewVelocity(currentVelocity, projectile.drag, predictInterval);
                nextPosition = currentPosition + currentVelocity * predictInterval;

                float overlap = Vector3.Distance(currentPosition, nextPosition) * RAY_OVERLAP;
                estimatedTime += predictInterval;

                if (Physics.Raycast(currentPosition, currentVelocity.normalized, out RaycastHit hit, overlap, groundLayer))
                {
                    UpdateLine(i, i - 1, hit.point);
                    ShowHitMarker(hit);
                    break;
                }

                HideHitMarker();
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
            trajectoryLine.enabled = state;
        }

        private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float predictInterval)
        {
            velocity += Physics.gravity * predictInterval;
            velocity *= Mathf.Clamp01(1f - drag * predictInterval);
            return velocity;
        }

        private void ShowHitMarker(RaycastHit hit)
        {
            hitMarker.gameObject.SetActive(true);
            hitMarker.position = hit.point + hit.normal * hitMarkerGroundOffset;
            hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
        }

        private void HideHitMarker()
        {
            hitMarker.gameObject.SetActive(false);
        }

        private IEnumerator StampMarker()
        {
            stampedMarker.gameObject.SetActive(true);
            stampedMarker.position = hitMarker.position;
            stampedMarker.rotation = hitMarker.rotation;

            yield return new WaitForSeconds(estimatedTime);

            stampedMarker.gameObject.SetActive(false);
            stampCoroutine = null;
        }
    }
}
