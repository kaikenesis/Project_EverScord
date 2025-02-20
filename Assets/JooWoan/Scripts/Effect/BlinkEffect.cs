using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

namespace EverScord.Effects
{
    public class BlinkEffect : MonoBehaviour
    {
        private const float DEFAULT_DURATION = 0.08f;
        private const float DEFAULT_INTENSITY = 0.7f;

        [SerializeField, ColorUsage(true, true)] private Color blinkColor;
        [SerializeField] private float blinkIntensity;
        [SerializeField] private float blinkDuration;

        private static Texture defaultTexture;
        private static int emissionMapID, emissionColorID;
        private const string EMISSION_KEYWORD = "_EMISSION";

        private MonoBehaviour blinkTarget;
        private Renderer[] renderers;
        private Coroutine blinkCoroutine;
        private MaterialPropertyBlock mpb;
        private IDictionary<(int, int), (Texture, Color)> emissionDict;

        public static BlinkEffect Create(MonoBehaviour target, float duration = DEFAULT_DURATION, float intensity = DEFAULT_INTENSITY, Color color = default)
        {
            if (!target)
            {
                Debug.LogWarning($"Blink Effect: Failed to find target.");
                return null;
            }

            if (color == default)
                color = Color.white;

            BlinkEffect blinkEffect = target.AddComponent<BlinkEffect>();
            blinkEffect.Init(target, duration, intensity, color);

            return blinkEffect;
        }

        public static Texture GetDefaultTexture()
        {
            if (defaultTexture != null)
                return defaultTexture;

            var texture = new Texture2D(4, 4);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                    texture.SetPixel(x, y, Color.white);
            }

            texture.Apply(true, true);
            defaultTexture = texture;

            return defaultTexture;
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
            blinkTarget = target;
            blinkDuration = duration;
            blinkIntensity = intensity;
            blinkColor = color;

            renderers = target.GetComponentsInChildren<Renderer>();
            mpb = new MaterialPropertyBlock();
            emissionDict = new Dictionary<(int, int), (Texture, Color)>();

            emissionMapID = Shader.PropertyToID("_EmissionMap");
            emissionColorID = Shader.PropertyToID("_EmissionColor");

            SetMaterialSettings();
        }

        public void Blink()
        {
            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);

            blinkCoroutine = StartCoroutine(StartBlink());
        }

        private void SetMaterialSettings()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                {
                    renderers[i].sharedMaterials[j].EnableKeyword(EMISSION_KEYWORD);
                    renderers[i].sharedMaterials[j].globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                }
            }
        }

        public void SetMaterialColors(Color color, bool isReset = false)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                {
                    renderers[i].GetPropertyBlock(mpb, j);

                    if (renderers[i].sharedMaterials[j].HasTexture(emissionMapID))
                    {
                        Texture texture = renderers[i].sharedMaterials[j].GetTexture(emissionMapID);

                        if (texture != null)
                        {
                            if (!emissionDict.ContainsKey((j, i)))
                            {
                                emissionDict[(j, i)] = (
                                    renderers[i].sharedMaterials[j].GetTexture(emissionMapID),
                                    renderers[i].sharedMaterials[j].GetColor(emissionColorID)
                                );
                            }

                            if (isReset)
                            {
                                mpb.SetTexture(emissionMapID, emissionDict[(j, i)].Item1);
                                mpb.SetColor(emissionColorID, emissionDict[(j, i)].Item2);
                            }
                            else
                                mpb.SetTexture(emissionMapID, GetDefaultTexture());
                        }
                    }

                    if (!isReset)
                        mpb.SetColor(emissionColorID, color);

                    renderers[i].SetPropertyBlock(mpb, j);
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
                intensity = (blinkProgress * blinkIntensity);

                SetMaterialColors(blinkColor * intensity);
                yield return null;
            }

            SetMaterialColors(default, true);
            blinkCoroutine = null;
        }
    }
}
