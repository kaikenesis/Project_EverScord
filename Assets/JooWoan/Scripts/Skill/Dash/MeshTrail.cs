using System.Collections;
using System.Collections.Generic;
using EverScord.Effects;
using EverScord.Pool;
using UnityEngine;
using UnityEngine.Rendering;

namespace EverScord.Skill
{
    public class MeshTrail
    {
        private const float SHADER_VAR_FADE = 0.05f;
        private const string SHADER_VAR_REF = "_Alpha";

        private List<SkinnedMeshRenderer> skinnedMeshRenderers = new();
        private Material trailMat;
        private DashSkillAction skillAction;
        private WaitForSeconds fadeWait, trailRefreshWait;
        private float trailRefreshRate, trailDestroyDelay, fadeRate;
        private readonly string DUMMY_ASSET_GUID;

        public MeshTrail(Transform characterTransform, DashSkillAction skillAction)
        {
            this.skillAction        = skillAction;
            trailRefreshRate        = skillAction.Skill.TrailRefreshRate;
            fadeRate                = skillAction.Skill.TrailFadeRate;
            trailMat                = skillAction.Skill.TrailMAT;

            trailDestroyDelay       = (1f / SHADER_VAR_FADE * fadeRate) + 0.01f;
            DUMMY_ASSET_GUID        = AssetReferenceManager.MeshTrailDummy_ID;

            fadeWait                = new WaitForSeconds(fadeRate);
            trailRefreshWait        = new WaitForSeconds(trailRefreshRate);

            InitSkinRenderers(characterTransform);
        }

        private void InitSkinRenderers(Transform characterTransform)
        {
            foreach (Transform bodyPart in characterTransform)
            {
                if (!bodyPart.gameObject.activeSelf)
                    continue;
                
                SkinnedMeshRenderer skinRenderer = bodyPart.GetComponent<SkinnedMeshRenderer>();

                if (skinRenderer)
                    skinnedMeshRenderers.Add(skinRenderer);
            }
        }

        public IEnumerator ActivateTrail(float duration)
        {
            if (skinnedMeshRenderers.Count == 0)
                yield break;

            while (duration > 0)
            {
                duration -= trailRefreshRate;

                for (int i = 0; i < skinnedMeshRenderers.Count; i++)
                {
                    MeshTrailDummy dummy = ResourceManager.Instance.GetFromPool(DUMMY_ASSET_GUID) as MeshTrailDummy;

                    if (dummy == null)
                    {
                        Debug.LogWarning("Dash Skill : Dummy has gone missing...");
                        continue;
                    }

                    dummy.transform.SetPositionAndRotation(
                        skinnedMeshRenderers[i].transform.position,
                        skinnedMeshRenderers[i].transform.rotation
                    );

                    skinnedMeshRenderers[i].BakeMesh(dummy.DummyMesh);
                    dummy.DummyMeshFilter.mesh = dummy.DummyMesh;

                    int matCount = skinnedMeshRenderers[i].materials.Length;
                    Material[] matArr = new Material[matCount];

                    for (int j = 0; j < matCount; j++)
                        matArr[j] = trailMat;

                    dummy.DummyMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                    dummy.DummyMeshRenderer.materials = matArr;

                    if (matArr.Length > 0)
                        skillAction.StartCoroutine(FadeTrail(dummy.DummyMeshRenderer.materials));

                    skillAction.StartCoroutine(DestroyDummy(dummy));
                }
                
                yield return trailRefreshWait;
            }
        }

        public IEnumerator DestroyDummy(IPoolable pooledDummy)
        {
            yield return new WaitForSeconds(trailDestroyDelay);
            ResourceManager.Instance.ReturnToPool(pooledDummy, DUMMY_ASSET_GUID);
        }

        public IEnumerator FadeTrail(Material[] matArr)
        {
            float valueToAnimate = matArr[0].GetFloat(SHADER_VAR_REF);

            while (valueToAnimate > 0)
            {
                valueToAnimate -= SHADER_VAR_FADE;

                for (int i = 0; i < matArr.Length; i++)
                    matArr[i].SetFloat(SHADER_VAR_REF, valueToAnimate);

                yield return fadeWait;
            }
        }
    }
}
