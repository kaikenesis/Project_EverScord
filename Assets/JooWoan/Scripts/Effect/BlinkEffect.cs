using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EverScord.Effects
{
    public class BlinkEffect : MonoBehaviour
    {
        private const float DEFAULT_DURATION = 0.08f;
        private const float DEFAULT_INTENSITY = 0.7f;
        private const int BLINK_REPEAT = 3;

        public BlinkEffectInfo BlinkInfo => blinkInfo;
        private BlinkEffectInfo blinkInfo;
        private BlinkEffectInfo? changedBlinkInfo = null;

        private static Material whiteMat;
        private static Texture defaultTexture;
        private static int emissionMapID, emissionColorID;
        private const string EMISSION_KEYWORD = "_EMISSION";

        private Transform blinkTarget;
        private Renderer[] renderers;
        private Coroutine blinkCoroutine, blinkLoopCoroutine;
        private MaterialPropertyBlock mpb;
        private IDictionary<(int, int), Texture> textureDict;
        private IDictionary<(int, int), Color> colorDict;
        private Color? currentDefaultColor;

        private List<Material> originalParticleMats;
        private bool isParticle = false;

        public static BlinkEffect Create(ParticleSystem target)
        {
            BlinkEffect blinkEffect = target.transform.gameObject.AddComponent<BlinkEffect>();
            blinkEffect.isParticle = true;

            return blinkEffect;
        }

        public static BlinkEffect Create(Transform target, BlinkEffectInfo info)
        {
            return Create(target.transform, info.BlinkDuration, info.BlinkIntensity, info.BlinkColor);
        }

        public static BlinkEffect Create(GameObject target, float duration = DEFAULT_DURATION, float intensity = DEFAULT_INTENSITY, Color color = default)
        {
            return Create(target.transform, duration, intensity, color);
        }

        public static BlinkEffect Create(MonoBehaviour target, float duration = DEFAULT_DURATION, float intensity = DEFAULT_INTENSITY, Color color = default)
        {
            return Create(target.transform, duration, intensity, color);
        }

        public static BlinkEffect Create(Transform target, float duration = DEFAULT_DURATION, float intensity = DEFAULT_INTENSITY, Color color = default)
        {
            if (target.TryGetComponent(out ParticleSystem particle))
            {
                Debug.Log($"Wrong Create Method for BlinkEffect, this contains a ParticleSystem: {target.name}");
                return Create(particle);
            }

            if (!target)
            {
                Debug.LogWarning($"Blink Effect: Failed to find target.");
                return null;
            }

            if (color == default)
                color = Color.white;

            BlinkEffect blinkEffect = target.gameObject.AddComponent<BlinkEffect>();
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
            return defaultTexture = texture;
        }

        public static Material GetDefaultMat()
        {
            if (whiteMat != null)
                return whiteMat;

            return whiteMat = ResourceManager.Instance.GetAsset<Material>(AssetReferenceManager.BlinkWhiteMat_ID);
        }

        void Awake()
        {
            if (!blinkTarget)
                Init(transform, blinkInfo.BlinkDuration, blinkInfo.BlinkIntensity, blinkInfo.BlinkColor);
        }

        void OnDisable()
        {
            if (!isParticle)
                SetMaterialColors(default, true);
            else
                ClearParticleBlink();
        }

        private void Init(Transform target, float duration, float intensity, Color color)
        {
            blinkTarget = target;

            blinkInfo = new BlinkEffectInfo()
            {
                BlinkDuration = duration,
                BlinkIntensity = intensity,
                BlinkColor = color
            };

            renderers = target.GetComponentsInChildren<Renderer>();
            mpb = new MaterialPropertyBlock();

            textureDict = new Dictionary<(int, int), Texture>();
            colorDict = new Dictionary<(int, int), Color>();

            emissionMapID = Shader.PropertyToID("_EmissionMap");
            emissionColorID = Shader.PropertyToID("_EmissionColor");

            SetMaterialSettings();
        }

        public void InitParticles(ParticleSystem target)
        {
            blinkTarget = target.gameObject.transform;
            ParticleSystemRenderer[] particleRenderers = blinkTarget.GetComponentsInChildren<ParticleSystemRenderer>();

            List<Renderer> temp = new();
            originalParticleMats = new();

            foreach (var renderer in particleRenderers)
            {
                if (!renderer.enabled)
                    continue;

                if (renderer.renderMode != ParticleSystemRenderMode.Mesh)
                    continue;

                if (renderer.mesh == null)
                    continue;

                if (renderer.tag != ConstStrings.TAG_PARTICLE_BLINKABLE)
                    continue;

                temp.Add(renderer);
                originalParticleMats.Add(new Material(renderer.sharedMaterial));
            }

            renderers = temp.ToArray();
        }

        public void Blink()
        {
            if (!gameObject.activeSelf)
                return;

            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);

            if (!isParticle)
                blinkCoroutine = StartCoroutine(StartBlink());
            else 
            {
                ClearParticleBlink();
                blinkCoroutine = StartCoroutine(FlickerBlink());
            }
        }
        
        public void LoopBlink(bool state)
        {
            if (blinkLoopCoroutine != null)
                StopCoroutine(blinkLoopCoroutine);

            if (!state)
            {
                SetMaterialColors(default, true);
                return;
            }

            if (!gameObject.activeSelf)
                return;

            blinkLoopCoroutine = StartCoroutine(StartBlink(true));
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
                            if (!textureDict.ContainsKey((j, i)))
                                textureDict[(j, i)] = renderers[i].sharedMaterials[j].GetTexture(emissionMapID);

                            if (isReset)
                                mpb.SetTexture(emissionMapID, textureDict[(j, i)]);
                            else
                                mpb.SetTexture(emissionMapID, GetDefaultTexture());
                        }
                    }

                    if (!colorDict.ContainsKey((j, i)))
                        colorDict[(j, i)] = renderers[i].sharedMaterials[j].GetColor(emissionColorID);
                    
                    if (isReset)
                        mpb.SetColor(emissionColorID, colorDict[(j, i)]);
                    else
                        mpb.SetColor(emissionColorID, color);

                    renderers[i].SetPropertyBlock(mpb, j);
                }
            }
        }

        private IEnumerator StartBlink(bool isLoop = false)
        {
            BlinkEffectInfo info = changedBlinkInfo != null ? (BlinkEffectInfo)changedBlinkInfo : blinkInfo;

            do
            {
                float leftTime = info.BlinkDuration;
                float blinkProgress;
                float intensity;

                while (leftTime >= 0)
                {
                    leftTime -= Time.deltaTime;

                    blinkProgress = Mathf.Clamp01(leftTime / info.BlinkDuration);
                    intensity = blinkProgress * info.BlinkIntensity;

                    SetMaterialColors(info.BlinkColor * intensity);
                    yield return null;
                }
            } while(isLoop);

            if (currentDefaultColor != null)
                SetMaterialColors((Color)currentDefaultColor);
            else
                SetMaterialColors(default, true);
            
            blinkCoroutine = null;
            changedBlinkInfo = null;
        }

        private IEnumerator FlickerBlink()
        {
            int blinkCount = BLINK_REPEAT;
            WaitForSeconds waitblink = new WaitForSeconds(0.01f);

            for (int i = 0; i < blinkCount; i++)
            {
                for (int j = 0; j < renderers.Length; j++)
                    renderers[j].sharedMaterial = GetDefaultMat();

                yield return waitblink;

                for (int j = 0; j < renderers.Length; j++)
                    renderers[j].sharedMaterial = originalParticleMats[j];

                yield return waitblink;
            }
        }

        private void ClearParticleBlink()
        {
            if (originalParticleMats == null || originalParticleMats.Count == 0)
                return;
            
            for (int j = 0; j < renderers.Length; j++)
                renderers[j].sharedMaterial = originalParticleMats[j];
        }

        public void ChangeBlinkTemporarily(BlinkEffectInfo info)
        {
            changedBlinkInfo = info;
        }

        public void SetDefaultColor(Color? color = null)
        {
            currentDefaultColor = color;
        }
    }

    [System.Serializable]
    public struct BlinkEffectInfo
    {
        [SerializeField, ColorUsage(true, true)] public Color BlinkColor;
        [SerializeField] public float BlinkIntensity;
        [SerializeField] public float BlinkDuration;
    }
}
