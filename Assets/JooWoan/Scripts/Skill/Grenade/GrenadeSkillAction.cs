using System.Collections;
using UnityEngine;
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
        private Transform startPoint, stampedMarker;
        private LineRenderer trajectoryLine;
        private Camera cam;
        private Coroutine skillCoroutine;
        private const float RAY_OVERLAP = 1.2f;
        private float force;
        private bool hasActivated = false;

        public bool IsUsingSkill
        {
            get { return skillCoroutine != null; }
        }

        public void Init(CharacterControl activator)
        {
            if (this.activator != null)
                return;

            this.activator = activator;

            cam = activator.CameraControl.Cam;
            trajectoryLine = GetComponent<LineRenderer>();

            stampedMarker = Instantiate(hitMarker, transform);
            stampedMarker.gameObject.SetActive(false);
            
            startPoint = Instantiate(grenadeStartPoint, activator.transform).transform;

            HideHitMarker();
            SetLineVisibility(false);
        }

        public void Activate(EJob ejob)
        {
            hasActivated = !hasActivated;

            if (!hasActivated)
            {
                StopCoroutine(skillCoroutine);
                skillCoroutine = null;

                HideHitMarker();
                SetLineVisibility(false);
                activator.PlayerUIControl.SetAimCursor(true);
                return;
            }

            SetLineVisibility(true);
            activator.PlayerUIControl.SetAimCursor(false);

            skillCoroutine = StartCoroutine(ActivateSkill());
        }

        private IEnumerator ActivateSkill()
        {
            while (hasActivated)
            {
                SetForce(); 
                Predict();
                Fire();

                yield return null;
            }
        }

        private void SetForce()
        {
            Ray ray = cam.ScreenPointToRay(activator.PlayerInputInfo.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                return;

            Vector3 startPos = new Vector3(startPoint.position.x, hit.point.y, startPoint.position.z);
            force = Vector3.Distance(hit.point, startPos);
        }

        private void Fire()
        {
            if (!activator.PlayerInputInfo.pressedLeftMouseButton)
                return;
            
            Rigidbody thrownObject = Instantiate(projectile, startPoint.position, Quaternion.identity);
            thrownObject.AddForce(startPoint.forward * force, ForceMode.Impulse);

            StampMarker();
        }

        private void Predict()
        {
            Vector3 currentVelocity = startPoint.forward * (force / projectile.mass);
            Vector3 currentPosition = startPoint.position;
            Vector3 nextPosition;

            UpdateLine(maxPoints, 0, currentPosition);

            for (int i = 1; i < maxPoints; i++)
            {
                currentVelocity = CalculateNewVelocity(currentVelocity, projectile.drag, predictInterval);
                nextPosition = currentPosition + currentVelocity * predictInterval;

                float overlap = Vector3.Distance(currentPosition, nextPosition) * RAY_OVERLAP;

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

        private void StampMarker()
        {
            stampedMarker.gameObject.SetActive(true);
            stampedMarker.position = hitMarker.position;
            stampedMarker.rotation = hitMarker.rotation;
        }
    }
}
