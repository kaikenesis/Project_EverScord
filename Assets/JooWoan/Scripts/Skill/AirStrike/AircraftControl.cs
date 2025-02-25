using System.Collections;
using UnityEngine;

namespace EverScord.Skill
{
    public class AircraftControl : MonoBehaviour
    {
        private Light lightSource;
        private Vector3 lightDir;
        private Coroutine travelCoroutine;
        private float aircraftHeight;
        private float totalTravelDistance;
        private float travelSpeed;
        private float startDelay;

        void Awake()
        {
            gameObject.SetActive(false);

            aircraftHeight = transform.position.y;
            lightSource = GameObject.FindGameObjectWithTag(ConstStrings.TAG_MAINLIGHT).GetComponent<Light>();

            if (lightSource == null)
            {
                Debug.LogWarning("Failed to find main light source from scene");
                return;
            }

            lightDir = lightSource.transform.forward;
        }

        public void Init(float totalTravelDistance, float travelSpeed, float startDelay = 0f)
        {
            this.totalTravelDistance = totalTravelDistance;
            this.travelSpeed = travelSpeed;
            this.startDelay = startDelay;
        }

        public void EnableAircraft(Vector3 shadowPoint, Vector3 moveDir)
        {
            Vector3 wayPoint = PositionRelativeToShadow(shadowPoint);

            SetRotation(moveDir);

            if (travelCoroutine != null)
                StopCoroutine(travelCoroutine);

            gameObject.SetActive(true);
            travelCoroutine = StartCoroutine(MoveAircraft(moveDir, wayPoint, startDelay));
        }
        
        private Vector3 PositionRelativeToShadow(Vector3 shadowPoint)
        {
            #region Aircraft Position Equation
            /*
                # Goal: Set the aircraft shadow at a specific position.

                V(t)         : Final aircraft position 
                V(t).y       : Aircraft height
                ShadowPoint  : Shadow position

                V(t) = ShadowPoint + Time * -LightDirection

                V(t).y = ShadowPoint.y + Time * -LightDirection.y
                (V(t).y - ShadowPoint.y) = Time * -LightDirection.y

                Time = (V(t).y - ShadowPoint.y) / -LightDirection.y
            */
            #endregion

            if (lightDir.y == 0)
            {
                Debug.LogWarning("Failed to set aircraft position");
                return transform.position;
            }

            float time = (aircraftHeight - shadowPoint.y) / -lightDir.y;
            return shadowPoint + time * -lightDir;
        }

        private void SetRotation(Vector3 moveDir)
        {
            Quaternion rotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = rotation;
        }

        private IEnumerator MoveAircraft(Vector3 moveDir, Vector3 wayPoint, float startDelay = 0f)
        {
            float halfDistance = totalTravelDistance * 0.5f;

            Vector3 startPoint = wayPoint - moveDir * halfDistance;
            Vector3 endPoint = wayPoint + moveDir * halfDistance;

            transform.position = startPoint;

            yield return new WaitForSeconds(startDelay);

            while (Vector3.Distance(transform.position, endPoint) > 1f)
            {
                transform.position += moveDir * travelSpeed * Time.deltaTime;
                yield return null;
            }

            gameObject.SetActive(false);
            travelCoroutine = null;
        }
    }
}
