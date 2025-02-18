using System.Collections;
using UnityEngine;
using EverScord.Character;
using Unity.VisualScripting;

namespace EverScord.Skill
{
    public class TrajectoryPredictor
    {
        private const float TRAJECTORY_STEP = 0.02f;
        private const float LINE_WIDTH = 0.2f;

        // -Physics.gravity.y;
        private readonly float GRAVITY; 

        private CharacterControl activator;
        private Transform throwPoint;
        private bool displayPath = false;

        private Camera cam;
        private LineRenderer trajectoryLine;
        private SkillMarker skillMarker;
        public SkillMarker MarkerControl => skillMarker;

        private Vector3[] vertexPositions;
        private Vector3 throwDirection, thrownPosition, groundDirection;
        private float trajectoryAngle, trajectoryHeight;
        private float initialVelocity, estimatedTime;
        private int totalVertices;

        public Vector3 ThrownPosition => thrownPosition;
        public Vector3 GroundDirection => groundDirection;
        public float InitialVelocity => initialVelocity;
        public float TrajectoryAngle => trajectoryAngle;
        public float EstimatedTime => estimatedTime;
        public bool IsThrownObjectMoving { get; private set; }

        public TrajectoryPredictor(CharacterControl activator, Transform skillTransform, ThrowSkill throwSkill)
        {
            this.activator  = activator;
            cam             = activator.CameraControl.Cam;

            displayPath     = throwSkill.DisplayTrajectory;
            GRAVITY         = throwSkill.Gravity;

            Material lineMat = ResourceManager.Instance
                .GetAsset<Material>(ConstStrings.KEY_TRAJECTORYLINE_MAT);

            trajectoryLine               = skillTransform.AddComponent<LineRenderer>();
            trajectoryLine.textureMode   = LineTextureMode.Stretch;
            trajectoryLine.startWidth    = LINE_WIDTH;
            trajectoryLine.endWidth      = LINE_WIDTH;
            trajectoryLine.material      = lineMat;
            trajectoryLine.useWorldSpace = true;

            throwPoint = Object.Instantiate(throwSkill.ThrowPoint, activator.transform).transform;

            skillMarker = new SkillMarker(activator, skillTransform, throwSkill.DestinationMarker);
            skillMarker.Set(false);
            skillMarker.SetStamped(false);

            SetPathVisibility(false);
        }

        public IEnumerator Activate()
        {
            skillMarker.Set(true);
            SetPathVisibility(displayPath);

            while (!IsThrownObjectMoving)
            {
                Ray ray = cam.ScreenPointToRay(activator.PlayerInputInfo.mousePosition);

                if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GameManager.GroundLayer))
                    yield return null;

                throwDirection = hit.point - throwPoint.position;
                groundDirection = new Vector3(throwDirection.x, 0, throwDirection.z);
                Vector3 targetPos = new Vector3(groundDirection.magnitude, throwDirection.y, 0);

                groundDirection.Normalize();

                CalculatePath(targetPos);
                DrawPath();

                yield return null;
            }

            SetPathVisibility(false);
        }

        public void Exit()
        {
            skillMarker.Set(false);
            SetPathVisibility(false);
        }

        public IEnumerator ThrowObject(Transform thrownObject)
        {
            IsThrownObjectMoving = true;

            skillMarker.Set(false);
            SetPathVisibility(false);
            
            estimatedTime += 0.1f;
            skillMarker.Stamp(estimatedTime);

            Vector3 position    = thrownPosition;
            Vector3 direction   = groundDirection;
            float velocity      = initialVelocity;
            float angle         = trajectoryAngle;

            for (float t = 0; t <= estimatedTime; t += Time.deltaTime)
            {
                if (thrownObject == null)
                    break;
                
                thrownObject.position = GetProjectilePosition(position, direction, velocity, angle, t);
                yield return null;
            }

            IsThrownObjectMoving = false;
        }

        private void CalculatePath(Vector3 targetPos)
        {
            trajectoryHeight = targetPos.y + targetPos.magnitude * 0.5f;
            trajectoryHeight = Mathf.Max(0.01f, trajectoryHeight);

            float a = -0.5f * GRAVITY;
            float b = Mathf.Sqrt(2 * GRAVITY * trajectoryHeight);
            float c = -targetPos.y;

            float tplus = QuadraticEquation(a, b, c, 1);
            float tmin  = QuadraticEquation(a, b, c, -1);

            estimatedTime   = Mathf.Max(tplus, tmin);
            trajectoryAngle = Mathf.Atan2(b * estimatedTime, targetPos.x);
            initialVelocity = b / Mathf.Sin(trajectoryAngle);

            thrownPosition = throwPoint.position;
        }

        private float QuadraticEquation(float a, float b, float c, float sign)
        {
            // tplus = (-b + sqrt(b^2 - 4ac)) / 2a
            // tmin  = (-b - sqrt(b^2 - 4ac)) / 2a

            return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
        }

        private void DrawPath()
        {
            totalVertices = (int)(estimatedTime / TRAJECTORY_STEP) + 2;
            vertexPositions = new Vector3[totalVertices];
            trajectoryLine.positionCount = totalVertices;

            for (int i = 0; i < totalVertices - 1; i++)
                vertexPositions[i] = GetProjectilePosition(thrownPosition, groundDirection, initialVelocity, trajectoryAngle, i * TRAJECTORY_STEP);

            vertexPositions[totalVertices - 1] = GetProjectilePosition(thrownPosition, groundDirection, initialVelocity, trajectoryAngle, estimatedTime);
            trajectoryLine.SetPositions(vertexPositions);

            skillMarker.Move(vertexPositions[totalVertices - 1]);
        }

        private Vector3 GetProjectilePosition(Vector3 throwPosition, Vector3 direction, float velocity, float angle, float elapsedTime)
        {
            (float, float) axis = GetProjectileXYAxis(velocity, angle, elapsedTime);
            return throwPosition + (direction * axis.Item1) + (Vector3.up * axis.Item2);
        }

        private (float, float) GetProjectileXYAxis(float velocity, float angle, float elapsedTime)
        {
            #region Projectile Movement Equation
            /*
                x:  x axis position
                y:  y axis position
                v0: initial velocity
                t:  time
                0:  initial angle
                h:  max height
                g:  gravity

                If we know the target x and y,
                we can calculate:
                    1. t:  Time needed to reach the point
                    2. v0: Initial velocity of the projectile

                Formulas:
                y = v0 * t * sin0 - (1/2 * g * t^2)
                x = v0 * t * cos0

                x = v0 * t * cos0
                -> t = (x / v0 * cos0)
                -> y = v0 * (x / v0 * cos0) * sin0 - (1/2 * g * (x / v0 * cos0)^2)
                -> y = x / cos0 * sin0 - (1/2 * g * (x / v0 * cos0)^2)
                -> y = x * tan0 - (1/2 * g * (x / v0 * cos0)^2)
                ...
                -> v0 = sqrt( (x ^ 2) / (2 * x * sin0 * cos0 - 2 * y * cos0^2) )
            */
            #endregion

            float x = velocity * elapsedTime * Mathf.Cos(angle);
            float y = velocity * elapsedTime * Mathf.Sin(angle) - (0.5f * GRAVITY * elapsedTime * elapsedTime);
            return (x, y);
        }

        public void SetPathVisibility(bool state)
        {
            trajectoryLine.enabled = state;
        }

        public void SyncInfo(Vector3 thrownPosition, Vector3 groundDirection, float initialVelocity, float trajectoryAngle, float estimatedTime)
        {
            this.thrownPosition  = thrownPosition;
            this.groundDirection = groundDirection;
            this.initialVelocity = initialVelocity;
            this.trajectoryAngle = trajectoryAngle;
            this.estimatedTime   = estimatedTime;
        }
    }
}