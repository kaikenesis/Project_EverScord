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

        private CharacterControl activator;
        private Transform throwPoint;
        private float moveSpeed;
        private bool displayPath = false;

        private LineRenderer trajectoryLine;
        private Camera cam;
        private Vector3 throwDirection, groundDirection;
        private float trajectoryAngle, trajectoryHeight;
        private float initialVelocity, estimatedTime;

        public bool IsTargetMoving { get; private set; }

        public TrajectoryPredictor(CharacterControl activator, Transform skillTransform, Transform throwPoint, bool displayPath = true, float moveSpeed = 1f)
        {
            this.activator = activator;
            this.throwPoint = throwPoint;
            this.moveSpeed = moveSpeed;
            this.displayPath = displayPath;

            cam = activator.CameraControl.Cam;

            Material lineMat = ResourceManager.Instance
                .GetAsset<Material>(ConstStrings.KEY_TRAJECTORYLINE_MAT);

            trajectoryLine              = skillTransform.AddComponent<LineRenderer>();
            trajectoryLine.textureMode  = LineTextureMode.Stretch;
            trajectoryLine.startWidth   = LINE_WIDTH;
            trajectoryLine.endWidth     = LINE_WIDTH;
            trajectoryLine.material     = lineMat;

            SetPathVisibility(false);
        }

        public IEnumerator Activate()
        {
            SetPathVisibility(true);

            while (true)
            {
                Ray ray = cam.ScreenPointToRay(activator.PlayerInputInfo.mousePosition);

                if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GameManager.GroundLayer))
                    yield return null;

                throwDirection = hit.point - throwPoint.position;
                groundDirection = new Vector3(throwDirection.x, 0, throwDirection.z);
                Vector3 targetPos = new Vector3(groundDirection.magnitude, throwDirection.y, 0);

                groundDirection.Normalize();

                CalculatePath(targetPos);
                DrawPath(groundDirection, initialVelocity, trajectoryAngle, estimatedTime);

                yield return null;
            }
        }

        public IEnumerator ThrowTarget(GameObject throwingObject)
        {
            Transform target = Object.Instantiate(throwingObject).transform;

            IsTargetMoving = true;
            trajectoryAngle *= Mathf.Deg2Rad;

            for (float t = 0; t < estimatedTime; t += Time.deltaTime * moveSpeed)
            {
                if (target == null)
                    break;

                target.position = GetProjectilePosition(groundDirection, initialVelocity, trajectoryAngle, t);
                yield return null;
            }

            IsTargetMoving = false;
        }

        private void CalculatePath(Vector3 targetPos)
        {
            trajectoryHeight = targetPos.y + targetPos.magnitude * 0.5f;
            trajectoryHeight = Mathf.Max(0.01f, trajectoryHeight);

            float g = -Physics.gravity.y;

            float a = -0.5f * g;
            float b = Mathf.Sqrt(2 * g * trajectoryHeight);
            float c = -targetPos.y;

            float tplus = QuadraticEquation(a, b, c, 1);
            float tmin = QuadraticEquation(a, b, c, -1);

            estimatedTime = Mathf.Max(tplus, tmin);
            trajectoryAngle = Mathf.Atan2(b * estimatedTime, targetPos.x);
            initialVelocity = b / Mathf.Sin(trajectoryAngle);
        }

        private float QuadraticEquation(float a, float b, float c, float sign)
        {
            // tplus = (-b + sqrt(b^2 - 4ac)) / 2a
            // tmin  = (-b - sqrt(b^2 - 4ac)) / 2a

            return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
        }

        private void DrawPath(Vector3 direction, float velocity, float angle, float time)
        {
            if (!displayPath)
                return;

            angle *= Mathf.Deg2Rad;
            float step = Mathf.Max(0.01f, TRAJECTORY_STEP);

            trajectoryLine.positionCount = (int)(time / step) + 2;
            int index = 0;

            for (float t = 0; t < time; t += step)
            {
                trajectoryLine.SetPosition(index, GetProjectilePosition(direction, velocity, angle, t));
                index++;
            }

            trajectoryLine.SetPosition(index, GetProjectilePosition(direction, velocity, angle, time));
        }

        public void SetPathVisibility(bool state)
        {
            trajectoryLine.enabled = state;
        }

        private Vector3 GetProjectilePosition(Vector3 direction, float velocity, float angle, float elapsedTime)
        {
            (float, float) axis = GetProjectileXYAxis(velocity, angle, elapsedTime);
            return throwPoint.position + (direction * axis.Item1) + (Vector3.up * axis.Item2);
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

                x = v0 * t * cos0
                y = v0 * t * sin0 - (1/2 * g * t^2)
            */
            #endregion

            float x = velocity * elapsedTime * Mathf.Cos(angle);
            float y = velocity * elapsedTime * Mathf.Sin(angle) - (0.5f * -Physics.gravity.y * elapsedTime * elapsedTime);
            return (x, y);
        }
    }
}