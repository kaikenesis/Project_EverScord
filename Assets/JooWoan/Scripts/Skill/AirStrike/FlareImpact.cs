using System.Collections;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class FlareImpact : ThrowableImpact
    {
        private const float MARKER_DISPLAY_INTERVAL = 0.2f;
        private const float MARKER_DISPLAY_DISTANCE = 5f;
        private const int MARKER_COUNT = 3;

        private AirStrikeSkill airStrikeSkill;
        private AirStrikeAction airStrikeAction;
        private GameObject airStrikeMarker;
        private Vector3 thrownDirection;

        public override void Init(CharacterControl activator, ThrowSkillAction skillAction)
        {
            base.Init(activator, skillAction);

            airStrikeAction = skillAction as AirStrikeAction;
            airStrikeSkill  = skillAction.ThrowingSkill as AirStrikeSkill;
            airStrikeMarker = skillAction.ThrowingSkill.DestinationMarker;
            thrownDirection = skillAction.Predictor.GroundDirection;
        }

        public override void OnCollisionEnter(Collision collision)
        {
            if (!IsValidCollision(collision))
                return;

            thrower.StartCoroutine(PrepareStrike());
            Destroy(gameObject);
        }

        private IEnumerator PrepareStrike()
        {
            Vector3 destLeftDir  = Quaternion.Euler(0, -90, 0) * thrownDirection;
            Vector3 destRightDir = Quaternion.Euler(0, 90, 0) * thrownDirection;

            Vector3 strikeStartPos = transform.position + destRightDir * MARKER_DISPLAY_DISTANCE;
            Vector3 strikeEndPos   = transform.position + destLeftDir * MARKER_DISPLAY_DISTANCE;

            GameObject[] markers = new GameObject[MARKER_COUNT];
            Vector3[] markerPos = { transform.position, strikeStartPos, strikeEndPos };

            for (int i = 0; i < MARKER_COUNT; i++)
            {
                markers[i] = Instantiate(airStrikeMarker, CharacterSkill.SkillRoot);
                markers[i].transform.position = markerPos[i];

                CharacterSkill.SetMarkerColor(markers[i], airStrikeSkill.AirStrikeMarkerColor);

                if (i > 0)
                    markers[i].SetActive(false);
            }

            for (int i = 1; i < MARKER_COUNT; i++)
            {
                yield return new WaitForSeconds(MARKER_DISPLAY_INTERVAL);
                markers[i].SetActive(true);
            }

            yield return new WaitForSeconds(MARKER_DISPLAY_INTERVAL * 2);

            for (int i = 0; i < MARKER_COUNT; i++)
                CharacterSkill.StopEffectParticles(markers[i]);

            airStrikeAction.SetAirStrikePosition(strikeStartPos, strikeEndPos);
            onSkillActivated.Invoke();
        }
    }
}
