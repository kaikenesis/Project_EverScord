using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using EverScord;
using DG.Tweening;

public class SoundManager : Singleton<SoundManager>
{
    // 오디오 소스 풀링을 위한 딕셔너리
    [SerializeField] private int audioSourcePoolSize = 10;
    private List<AudioSource> audioSourcePool = new List<AudioSource>();

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private AudioSource currentBGM;

    private const float MIN_VOLUME_DB = -80f;

    protected override void Awake()
    {
        base.Awake();
        InitializeSoundManager();
    }

    private void Start()
    {
        LevelControl.OnLevelUpdated += ManageBGM;
    }

    private void ManageBGM(int curStageNum, bool bCoverScreen)
    {
        if (bCoverScreen == false)
            return;

        if ((curStageNum + 1) % 2 == 1)
            PlayBGM("InGameBGM01");
        else
            PlayBGM("InGameBGM02");
    }

    public void StopBGM(string soundName, float fadeOutTime)
    {
        StartCoroutine(FadeOutSound(currentBGM, fadeOutTime));
    }

    private void InitializeSoundManager()
    {
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            CreateAudioSource();
        }

        //LoadVolumeSettings();
    }

    private AudioSource CreateAudioSource()
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        audioSourcePool.Add(newSource);
        return newSource;
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        return CreateAudioSource();
    }

    public AudioSource PlaySound(string soundName, float volume = 1.0f, bool playOnly = false)
    {
        if (playOnly)
        {
            foreach (AudioSource asc in audioSourcePool)
            {
                if (asc.isPlaying && asc.clip.name == soundName)
                {
                    return asc;
                }
            }
        }
        AudioClip audioClip = ResourceManager.Instance.GetAsset<AudioClip>(soundName);
        audioClip.name = soundName;

        if (audioClip == null)
            return null;
        return PlaySound(audioClip, volume);
    }

    public AudioSource PlaySound(AudioClip clip, float volume)
    {        
        AudioSource source = GetAvailableAudioSource();
        source.clip = clip;
        source.loop = false;
        source.volume = volume;
        source.outputAudioMixerGroup = sfxMixerGroup;
        source.spatialBlend = 0;
        source.Play();
        return source;
    }

    // 3D 사운드 재생
    public AudioSource PlaySoundAtPosition(string soundName, Vector3 position, float volume = 1.0f, bool playOnly = false)
    {
        if (playOnly)
        {
            foreach (AudioSource asc in audioSourcePool)
            {
                if (asc.isPlaying && asc.clip.name == soundName)
                {
                    return asc;
                }
            }
        }
        AudioClip audioClip = ResourceManager.Instance.GetAsset<AudioClip>(soundName);
        AudioSource source = PlaySound(audioClip, volume);
        source.spatialBlend = 1;
        source.transform.position = position;
        return source;
    }

    public void PlayBGM(string soundName, float duration = 1.0f)
    {
        AudioClip audioClip = ResourceManager.Instance.GetAsset<AudioClip>(soundName);

        AudioSource source = GetAvailableAudioSource();
        source.clip = audioClip;
        source.volume = 0;
        source.loop = true;
        source.outputAudioMixerGroup = bgmMixerGroup;
        
        if (currentBGM != null && currentBGM.isPlaying)
        {
            StopCoroutine(nameof(FadeOutSound));
            StartCoroutine(FadeOutSound(currentBGM, duration));
        }

        currentBGM = source;
        source.Play();
        StartCoroutine(FadeInRoutine(source, 1, duration));
    }

    private IEnumerator FadeOutSound(AudioSource source, float duration)
    {
        float startVolume = source.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    private IEnumerator FadeInRoutine(AudioSource source, float targetVolume, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0, targetVolume, t / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    public void StopSound(string soundName)
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (source.isPlaying && source.clip.name == soundName)
            {
                StartCoroutine(FadeOutSound(source, 1f));
            }
        }
    }

    public void StopAllSounds()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            source.Stop();
        }
    }

    public void SetVolume(EAudioMixerType category, float volumePercent)
    {
        // 볼륨을 데시벨로 변환 (0-1 값을 사용)
        float volumeDB = volumePercent <= 0 ? MIN_VOLUME_DB : Mathf.Log10(volumePercent) * 20;

        audioMixer.SetFloat(category.ToString(), volumeDB);
        //SaveVolumeSettings(category, volumePercent);
    }

    private void SaveVolumeSettings(EAudioMixerType category, float volume)
    {
        PlayerPrefs.SetFloat(category.ToString() + "Volume", volume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        SetVolume(EAudioMixerType.Master, PlayerPrefs.GetFloat("MasterVolume", 1.0f));
        SetVolume(EAudioMixerType.BGM, PlayerPrefs.GetFloat("BGMVolume", 1.0f));
        SetVolume(EAudioMixerType.SFX, PlayerPrefs.GetFloat("SFXVolume", 1.0f));
    }

    public void ResetVolumeSettings()
    {
        PlayerPrefs.DeleteKey("MasterVolume");
        PlayerPrefs.DeleteKey("BGMVolume");
        PlayerPrefs.DeleteKey("SFXVolume");
        PlayerPrefs.DeleteKey("UIVolume");
        PlayerPrefs.Save();
    }
}