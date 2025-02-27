using UnityEngine;

public class V3DLaserProgressCustom : MonoBehaviour
{
    public bool changeAllMaxLength = true;
    public float maxLength = 32f;
    public float globalProgressSpeed = 1f;
    public float globalImpactProgressSpeed = 1f;
    public bool always = true;
    public bool colorizeAll = true;
    public Color finalColor;
    [Range(0.2f, 1.0f)]
    public float gammaLinear = 1f;
    public Light pointLight;
    public StartPointEffectControllerV3D startPointEffect;
    public SmartWaveParticlesControllerV3D smartWaveParticles;
    public SFXControllerV3D sfxcontroller;

    private float globalProgress;
    private float globalImpactProgress;
    private LaserLineV3D[] lls;
    private LightLineV3D[] lils;
    private Renderer[] renderers;

    private void Start()
    {
        globalProgress = 1f;
        globalImpactProgress = 1f;
        lls = GetComponentsInChildren<LaserLineV3D>(true);
        lils = GetComponentsInChildren<LightLineV3D>(true);
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    public void ChangeColor(Color color)
    {
        finalColor = color;
    }

    void Update()
    {
        // Control Gamma and Linear modes
        foreach (Renderer rend in renderers)
        {
            rend.material.SetFloat("_GammaLinear", gammaLinear);
        }

        // Sending global progress value to other scripts
        startPointEffect.SetGlobalProgress(globalProgress);
        startPointEffect.SetGlobalImpactProgress(globalImpactProgress);

        smartWaveParticles.SetGlobalProgress(globalProgress);

        // Color control of all child prefabs
        if (colorizeAll == true)
        {
            foreach (LightLineV3D lil in lils)
            {
                lil.SetFinalColor(finalColor);
            }
            startPointEffect.SetFinalColor(finalColor);
            
            foreach (Renderer rend in renderers)
            {
                rend.material.SetColor("_FinalColor", finalColor);
            }
        }

        // Overall progress control
        //if (meshRend != null)
        //{
        //    meshRend.material.SetColor("_EmissionColor", finalColor * meshRendPower);
        //}

        if (globalProgress < 1f)
        {
            globalProgress += Time.deltaTime * globalProgressSpeed;
        }

        if (globalImpactProgress < 1f)
        {
            globalImpactProgress += Time.deltaTime * globalImpactProgressSpeed;
        }

        if (always == true)
        {
            globalProgress = 0f;
        }

        foreach (LaserLineV3D ll in lls)
        {
            ll.SetGlobalProgress(globalProgress);
            ll.SetGlobalImpactProgress(globalImpactProgress);
            if (changeAllMaxLength == true)
            {
                ll.maxLength = maxLength;
            }
        }

        foreach (LightLineV3D lil in lils)
        {
            lil.SetGlobalProgress(globalProgress);
            lil.SetGlobalImpactProgress(globalImpactProgress);
            if (changeAllMaxLength == true)
            {
                lil.maxLength = maxLength;
            }
        }

        sfxcontroller.SetGlobalProgress(1f - globalProgress);
    }
}

