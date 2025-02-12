using System.Collections;
using System.Collections.Generic;
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
        private WaitForSeconds fadeWait;
        private float trailRefreshRate, trailDestroyDelay, fadeRate;

        public MeshTrail(Transform characterTransform, DashSkillAction skillAction)
        {
            this.skillAction        = skillAction;
            trailRefreshRate        = skillAction.Skill.TrailRefreshRate;
            fadeRate                = skillAction.Skill.TrailFadeRate;
            trailMat                = skillAction.Skill.TrailMAT;

            trailDestroyDelay       = (1f / SHADER_VAR_FADE * fadeRate) + 0.01f;

            fadeWait = new WaitForSeconds(fadeRate);
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
                    GameObject dummySkin = new GameObject();

                    dummySkin.transform.SetPositionAndRotation(
                        skinnedMeshRenderers[i].transform.position,
                        skinnedMeshRenderers[i].transform.rotation
                    );

                    MeshRenderer meshRenderer = dummySkin.AddComponent<MeshRenderer>();
                    MeshFilter meshFilter     = dummySkin.AddComponent<MeshFilter>();

                    Mesh mesh = new Mesh();
                    skinnedMeshRenderers[i].BakeMesh(mesh);
                    meshFilter.mesh = mesh;

                    int matCount = skinnedMeshRenderers[i].materials.Length;
                    Material[] matArr = new Material[matCount];

                    for (int j = 0; j < matCount; j++)
                        matArr[j] = trailMat;

                    meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                    meshRenderer.materials = matArr;

                    if (matArr.Length > 0)
                        skillAction.StartCoroutine(FadeTrail(meshRenderer.materials));

                    Object.Destroy(dummySkin, trailDestroyDelay);
                    Object.Destroy(mesh, trailDestroyDelay);
                }
                
                yield return new WaitForSeconds(trailRefreshRate);
            }
        }

        public IEnumerator FadeTrail(Material[] matArr)
        {
            float valueToAnimate = matArr[0].GetFloat(SHADER_VAR_REF);

            while (valueToAnimate > 0)
            {
                valueToAnimate -= SHADER_VAR_FADE;
                matArr[0].SetFloat(SHADER_VAR_REF, valueToAnimate);

                for (int i = 1; i < matArr.Length; i++)
                    matArr[i].SetFloat(SHADER_VAR_REF, valueToAnimate);

                yield return fadeWait;
            }
        }
    }
}
