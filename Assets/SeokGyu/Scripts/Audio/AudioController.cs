using UnityEngine;
using UnityEngine.Audio;

namespace EverScord
{
    public enum EAudioMixerType
    {
        Master,
        BGM,
        SFX,
        UI
    }

    public class AudioController : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        
        private bool[] isMute = new bool[3];
        private float[] audioVolumes = new float[3];

        private void SetAudioVolume(EAudioMixerType audioMixerType, float volume)
        {
            audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(volume) * 20);
        }

        private void SetAudioMute(EAudioMixerType audioMixerType)
        {
            int type = (int)audioMixerType;
            if (!isMute[type])
            {
                isMute[type] = true;
                audioMixer.GetFloat(audioMixerType.ToString(), out float curVolume);
                audioVolumes[type] = curVolume;
                SetAudioVolume(audioMixerType, 0.001f);
            }
            else
            {
                isMute[type] = false;
                SetAudioVolume(audioMixerType, audioVolumes[type]);
            }
        }

        public void ChangeMasterVolume(float volume)
        {
            SetAudioVolume(EAudioMixerType.Master, volume);
        }
    }
}
