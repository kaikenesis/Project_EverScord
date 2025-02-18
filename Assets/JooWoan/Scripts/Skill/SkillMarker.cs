using EverScord.Character;
using System.Collections;
using UnityEngine;

namespace EverScord.Skill
{
    public class SkillMarker
    {
        private const float HITMARKER_GROUND_OFFSET = 0.1f;

        public Transform Marker         {  get; private set; }
        public Transform StampedMarker  {  get; private set; }

        private CharacterControl activator;
        private Coroutine stampCoroutine = null;

        public SkillMarker(CharacterControl activator, Transform skillTransform, GameObject marker, GameObject stampedMarker = null)
        {
            this.activator = activator;
            Marker = Object.Instantiate(marker, skillTransform).transform;

            if (stampedMarker)
                StampedMarker = Object.Instantiate(stampedMarker, skillTransform).transform;
            else
                StampedMarker = Object.Instantiate(marker, skillTransform).transform;
        }

        public void Set(bool state)
        {
            Marker.gameObject.SetActive(state);
        }

        public void Move(Vector3 hitPoint)
        {
            Marker.position = hitPoint + new Vector3(0, HITMARKER_GROUND_OFFSET, 0);
            // marker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
        }

        public void SetStamped(bool state)
        {
            StampedMarker.gameObject.SetActive(state);
        }

        public void Stamp(float duration)
        {
            if (stampCoroutine != null)
                activator.StopCoroutine(stampCoroutine);

            stampCoroutine = activator.StartCoroutine(ProceedStamp(duration));
        }

        private IEnumerator ProceedStamp(float duration)
        {
            StampedMarker.position = Marker.position;
            StampedMarker.rotation = Marker.rotation;
            SetStamped(true);

            yield return new WaitForSeconds(duration);

            SetStamped(false);
        }

        public static void SetMarkerColor(GameObject marker, Color32 color)
        {
            ParticleSystem[] particles = marker.GetComponentsInChildren<ParticleSystem>();

            if (particles.Length == 0)
                return;

            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem.MainModule settings = particles[i].main;
                settings.startColor = new ParticleSystem.MinMaxGradient(color);
            }
        }
    }
}
