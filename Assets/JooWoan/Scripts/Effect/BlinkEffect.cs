using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

namespace EverScord.Effects
{
    public class BlinkEffect : MonoBehaviour
    {
        [SerializeField] private Color blinkColor;
        [SerializeField] private float blinkIntensity;
        [SerializeField] private float blinkDuration;

        private MonoBehaviour blinkTarget;
        private SkinnedMeshRenderer[] skinRenderers;
        private List<List<Color>> originalColors;
        private Coroutine blinkCoroutine;

        public static BlinkEffect Create(MonoBehaviour target, float duration, float intensity, Color color)
        {
            if (!target)
            {
                Debug.LogWarning($"Blink Effect: Failed to find target.");
                return null;
            }

            BlinkEffect blinkEffect = target.AddComponent<BlinkEffect>();
            blinkEffect.Init(target, duration, intensity, color);
            
            return blinkEffect;
        }

        void Awake()
        {
            if (!blinkTarget)
                Init(this, blinkDuration, blinkIntensity, blinkColor);
        }

        void OnDisable()
        {
            SetMaterialColors(default, true);
        }

        private void Init(MonoBehaviour target, float duration, float intensity, Color color)
        {
            blinkTarget     = target;
            blinkDuration   = duration;
            blinkIntensity  = intensity;
            blinkColor      = color;
            skinRenderers   = target.GetComponentsInChildren<SkinnedMeshRenderer>();

            CacheMaterialColors();
        }

        public void Blink()
        {
            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);
            
            blinkCoroutine = StartCoroutine(StartBlink());
        }

        private void CacheMaterialColors()
        {
            originalColors = new();

            for (int i = 0; i < skinRenderers.Length; i++)
            {
                originalColors.Add(new());

                for (int j = 0; j < skinRenderers[i].materials.Length; j++)
                    originalColors[i].Add(skinRenderers[i].materials[j].color);
            }
        }

        public void SetMaterialColors(Color color, bool isReset = false)
        {
            for (int i = 0; i < skinRenderers.Length; i++)
            {
                Color targetColor = color;

                for (int j = 0; j < skinRenderers[i].materials.Length; j++)
                {
                    if (isReset)
                        targetColor = originalColors[i][j];
                    
                    skinRenderers[i].materials[j].color = targetColor;
                }
            }
        }

        private IEnumerator StartBlink()
        {
            float leftTime = blinkDuration;
            float blinkProgress;
            float intensity;

            while (leftTime >= 0)
            {                
                leftTime -= Time.deltaTime;

                blinkProgress = Mathf.Clamp01(leftTime / blinkDuration);
                intensity = (blinkProgress * blinkIntensity) + 1f;
                
                SetMaterialColors(blinkColor * intensity);
                yield return null;
            }

            SetMaterialColors(default, true);
            blinkCoroutine = null;
        }
    }
}
