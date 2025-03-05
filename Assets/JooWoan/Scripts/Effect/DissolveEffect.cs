using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace EverScord.Effects
{
    public class DissolveEffect : MonoBehaviour
    {
        private static int baseMapID, metallicMapID, smoothnessID, normalMapID, occlusionMapID, emissionMapID, emissionColorID, splitValueID;
        private static Material dissolveMat;
        private static WaitForSeconds waitDissolveEnd = new WaitForSeconds(1f);
        private IDictionary<Material, Material> dissolveDict = new Dictionary<Material, Material>();
        private Renderer[] renderers;
        private List<Material[]> originalMats = new();

        public static DissolveEffect Create(MonoBehaviour target)
        {
            DissolveEffect dissolveEffect = target.gameObject.AddComponent<DissolveEffect>();
            dissolveEffect.Init();
            return dissolveEffect;
        }

        private void Init()
        {
            baseMapID       = Shader.PropertyToID("_BaseMap");
            metallicMapID   = Shader.PropertyToID("_MetallicGlossMap");
            normalMapID     = Shader.PropertyToID("_BumpMap");
            occlusionMapID  = Shader.PropertyToID("_OcclusionMap");
            emissionMapID   = Shader.PropertyToID("_EmissionMap");

            smoothnessID    = Shader.PropertyToID("_Metallic");
            emissionColorID = Shader.PropertyToID("_EmissionColor");
            splitValueID    = Shader.PropertyToID("_SplitValue");

            dissolveMat = ResourceManager.Instance.GetAsset<Material>(AssetReferenceManager.DissolveMat_ID);

            renderers = GetComponentsInChildren<Renderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] originalCopy = new Material[renderers[i].sharedMaterials.Length];

                for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                {
                    originalCopy[j] = renderers[i].sharedMaterials[j];

                    if (dissolveDict.ContainsKey(renderers[i].sharedMaterials[j]))
                        continue;
                    
                    Material copiedMat = new Material(dissolveMat);
                    SetTexture(i, j, copiedMat);

                    if (renderers[i].sharedMaterials[j].HasFloat(smoothnessID))
                        copiedMat.SetFloat(smoothnessID, renderers[i].sharedMaterials[j].GetFloat(smoothnessID));

                    if (renderers[i].sharedMaterials[j].HasColor(emissionColorID))
                        copiedMat.SetColor(emissionColorID, renderers[i].sharedMaterials[j].GetColor(emissionColorID));

                    dissolveDict[renderers[i].sharedMaterials[j]] = copiedMat;
                }

                originalMats.Add(originalCopy);
            }
        }

        void OnDisable()
        {
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].sharedMaterials = originalMats[i];
        }

        private void SetTexture(int i, int j, Material copiedMat)
        {
            int[] idArr = { baseMapID, metallicMapID, normalMapID, occlusionMapID, emissionMapID };

            for (int k = 0; k < idArr.Length; k++)
            {
                if (!renderers[i].sharedMaterials[j].HasTexture(idArr[k]))
                    continue;

                copiedMat.SetTexture(idArr[k], renderers[i].sharedMaterials[j].GetTexture(idArr[k]));
            }
        }

        public IEnumerator Dissolve(float duration)
        {
            float value = 1f;

            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] mats = renderers[i].sharedMaterials;

                for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                {
                    if (!dissolveDict.ContainsKey(mats[j]))
                        continue;
                    
                    mats[j] = dissolveDict[originalMats[i][j]];
                    mats[j].SetFloat(splitValueID, value);
                }

                renderers[i].sharedMaterials = mats;
            }

            for (float t = 0; t <= duration; t += Time.deltaTime)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                        renderers[i].sharedMaterials[j].SetFloat(splitValueID, value - t);
                }

                yield return null;
            }

            yield return waitDissolveEnd;

            for (int i = 0; i < renderers.Length; i++)
                renderers[i].sharedMaterials = originalMats[i];
        }
    }
}
