using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace EverScord.UI
{
    public static class VolumeAnimator
    {
        public static IEnumerator BlurBackground(DepthOfField depthOfField, float targetDistance, float currentDistance, float lerpSpeed)
        {
            if (!depthOfField)
                yield break;

            depthOfField.active = true;

            while (!Mathf.Approximately(currentDistance, targetDistance))
            {
                currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * lerpSpeed);
                depthOfField.focusDistance.value = currentDistance;
                yield return null;
            }

            depthOfField.focusDistance.value = targetDistance;
        }
    }
}
