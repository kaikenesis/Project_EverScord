using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using EverScord;

public class SoundManager : Singleton<SoundManager>
{
    // 오디오 소스 풀링을 위한 딕셔너리
    [SerializeField] private int audioSourcePoolSize = 10;
    private List<AudioSource> audioSourcePool = new List<AudioSource>();

    // 사운드 카테고리 믹서 참조
    [SerializeField] private AudioMixer audioMixer;

    // 카테고리별 볼륨 설정을 위한 믹서 그룹
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup uiMixerGroup;

    // 사운드 클립
    [System.Serializable]
    public class SoundClip
    {
        public string name;
        public AudioClip clip;
        public SoundCategory category;
        [Range(0f, 1f)] public float volume = 1f;
        public bool loop = false;
        [Range(0.1f, 3f)] public float pitch = 1f;
        [Range(0f, 1f)] public float spatialBlend = 0f; // 0: 2D, 1: 3D
    }

    public enum SoundCategory
    {
        BGM,
        SFX,
        UI
    }

    [SerializeField] private List<SoundClip> soundClips = new List<SoundClip>();
    private Dictionary<string, SoundClip> soundClipDict = new Dictionary<string, SoundClip>();

    // 현재 재생 중인 BGM의 오디오 소스 참조 (페이드 인/아웃 등을 위해)
    private AudioSource currentBGM;

    // 프리셋 볼륨 값
    private const float MIN_VOLUME_DB = -80f;

    protected override void Awake()
    {
        base.Awake();
        InitializeSoundManager();
    }

    private void InitializeSoundManager()
    {
        // 오디오 소스 풀 초기화
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            CreateAudioSource();
        }

        // 사운드 클립 딕셔너리 초기화
        foreach (SoundClip clip in soundClips)
        {
            if (!soundClipDict.ContainsKey(clip.name))
            {
                soundClipDict.Add(clip.name, clip);
            }
            else
            {
                Debug.LogWarning($"중복된 사운드 클립 이름이 있습니다: {clip.name}");
            }
        }

        // 초기 볼륨 설정 로드 (PlayerPrefs 활용 예시)
        LoadVolumeSettings();
    }

    private AudioSource CreateAudioSource()
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        audioSourcePool.Add(newSource);
        return newSource;
    }

    // 사용 가능한 오디오 소스 가져오기
    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // 모든 오디오 소스가 사용 중이면 새로 생성
        return CreateAudioSource();
    }

    // 사운드 재생 (이름으로)
    public AudioSource PlaySound(string soundName)
    {
        if (!soundClipDict.TryGetValue(soundName, out SoundClip clip))
        {
            Debug.LogWarning($"사운드 클립을 찾을 수 없습니다: {soundName}");
            return null;
        }

        return PlaySound(clip);
    }

    // 사운드 재생 (클립으로)
    public AudioSource PlaySound(SoundClip clip)
    {
        AudioSource source = GetAvailableAudioSource();
        source.clip = clip.clip;
        source.volume = clip.volume;
        source.loop = clip.loop;
        source.pitch = clip.pitch;
        source.spatialBlend = clip.spatialBlend;

        // 믹서 그룹 설정
        switch (clip.category)
        {
            case SoundCategory.BGM:
                source.outputAudioMixerGroup = bgmMixerGroup;
                if (currentBGM != null && currentBGM.isPlaying)
                {
                    StopCoroutine("FadeOutBGM");
                    StartCoroutine(FadeOutBGM(currentBGM, 1.0f));
                }
                currentBGM = source;
                break;
            case SoundCategory.SFX:
                source.outputAudioMixerGroup = sfxMixerGroup;
                break;
            case SoundCategory.UI:
                source.outputAudioMixerGroup = uiMixerGroup;
                break;
        }

        source.Play();
        return source;
    }

    // 3D 위치에서 사운드 재생
    public AudioSource PlaySoundAtPosition(string soundName, Vector3 position)
    {
        if (!soundClipDict.TryGetValue(soundName, out SoundClip clip))
        {
            Debug.Log($"사운드 클립을 찾을 수 없습니다: {soundName}");
            return null;
        }

        AudioSource source = PlaySound(clip);
        source.transform.position = position;
        return source;
    }

    // BGM 페이드 아웃
    private IEnumerator FadeOutBGM(AudioSource source, float duration)
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

    // BGM 페이드 인
    public void FadeInBGM(string soundName, float duration)
    {
        if (!soundClipDict.TryGetValue(soundName, out SoundClip clip))
        {
            Debug.Log($"사운드 클립을 찾을 수 없습니다: {soundName}");
            return;
        }

        AudioSource source = GetAvailableAudioSource();
        source.clip = clip.clip;
        source.volume = 0;
        source.loop = clip.loop;
        source.outputAudioMixerGroup = bgmMixerGroup;

        if (currentBGM != null && currentBGM.isPlaying)
        {
            StopCoroutine("FadeOutBGM");
            StartCoroutine(FadeOutBGM(currentBGM, duration));
        }

        currentBGM = source;
        source.Play();
        StartCoroutine(FadeInRoutine(source, clip.volume, duration));
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

    // 특정 사운드 정지
    public void StopSound(string soundName)
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (source.isPlaying && source.clip == soundClipDict[soundName].clip)
            {
                source.Stop();
            }
        }
    }

    // 모든 사운드 정지
    public void StopAllSounds()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            source.Stop();
        }
    }

    // 카테고리별 볼륨 설정
    public void SetVolume(SoundCategory category, float volumePercent)
    {
        // 볼륨을 데시벨로 변환 (0-1 값을 사용)
        float volumeDB = volumePercent <= 0 ? MIN_VOLUME_DB : Mathf.Log10(volumePercent) * 20;

        string paramName = "";
        switch (category)
        {
            case SoundCategory.BGM:
                paramName = "BGMVolume";
                break;
            case SoundCategory.SFX:
                paramName = "SFXVolume";
                break;
            case SoundCategory.UI:
                paramName = "UIVolume";
                break;
        }

        audioMixer.SetFloat(paramName, volumeDB);
        SaveVolumeSettings(category, volumePercent);
    }

    // 볼륨 설정 저장
    private void SaveVolumeSettings(SoundCategory category, float volume)
    {
        PlayerPrefs.SetFloat(category.ToString() + "Volume", volume);
        PlayerPrefs.Save();
    }

    // 볼륨 설정 로드
    private void LoadVolumeSettings()
    {
        SetVolume(SoundCategory.BGM, PlayerPrefs.GetFloat("BGMVolume", 1.0f));
        SetVolume(SoundCategory.SFX, PlayerPrefs.GetFloat("SFXVolume", 1.0f));
        SetVolume(SoundCategory.UI, PlayerPrefs.GetFloat("UIVolume", 1.0f));
    }

    public void ResetVolumeSettings()
    {
        // PlayerPrefs 데이터 삭제
        PlayerPrefs.DeleteKey("BGMVolume");
        PlayerPrefs.DeleteKey("SFXVolume");
        PlayerPrefs.DeleteKey("UIVolume");
        PlayerPrefs.Save();
    }

    // 사운드 클립 동적 추가
    public void AddSoundClip(AudioClip clip, string name, SoundCategory category, float volume = 1f, bool loop = false)
    {
        if (soundClipDict.ContainsKey(name))
        {
            Debug.Log($"이미 존재하는 사운드 이름입니다: {name}");
            return;
        }

        SoundClip newClip = new SoundClip
        {
            name = name,
            clip = clip,
            category = category,
            volume = volume,
            loop = loop
        };

        soundClips.Add(newClip);
        soundClipDict.Add(name, newClip);
    }
}