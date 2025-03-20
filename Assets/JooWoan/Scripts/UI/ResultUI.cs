using DG.Tweening;
using SciFiArsenal;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EverScord.UI
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField] private ResultSlot killSlot;
        [SerializeField] private ResultSlot damageSlot;
        [SerializeField] private ResultSlot healSlot;
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private GameObject counterAudioPrefab;
        [SerializeField] private int counterAudioSpawnCount;
        [SerializeField] private float minPlaybackInterval;

        private List<AudioSource> counterAudioList = new();
        private DOTweenAnimation[] tweens;

        void Awake()
        {
            tweens = GetComponentsInChildren<DOTweenAnimation>(true);
        }

        public void Init(int killCount, float dealtDamage, float dealtHeal, string nickname)
        {
            killSlot.Init(killCount, this);
            damageSlot.Init(dealtDamage, this);
            healSlot.Init(dealtHeal, this);
            nicknameText.text = nickname;

            for (int i = 0; i < counterAudioSpawnCount; i++)
            {
                AudioSource source = Instantiate(counterAudioPrefab, transform).GetComponent<AudioSource>();
                counterAudioList.Add(source);
            }
        }

        public void PlayTween()
        {
            for (int i = 0; i < tweens.Length; i++)
            {
                tweens[i].DORewind();
                tweens[i].DOPlay();
            }
        }

        public void PlayCounterSound()
        {
            for (int i = 0; i < counterAudioList.Count; i++)
            {
                if (counterAudioList[i].isPlaying || counterAudioList[i].time <= minPlaybackInterval)
                    continue;

                counterAudioList[i].Play();
                break;
            }
        }
    }
}
