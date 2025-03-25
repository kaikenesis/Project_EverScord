using DG.Tweening;
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
        [SerializeField] private AudioSource counterAudio;
        [SerializeField] private GameObject checkBox;
        [SerializeField] private float minPlaybackInterval;

        private DOTweenAnimation[] tweens;
        public int ViewID { get; private set; }

        void Awake()
        {
            checkBox.SetActive(false);
            tweens = GetComponentsInChildren<DOTweenAnimation>(true);
        }

        public void Init(int killCount, float dealtDamage, float dealtHeal, string nickname, int viewID)
        {
            killSlot.Init(killCount, counterAudio, minPlaybackInterval);
            damageSlot.Init(dealtDamage, counterAudio, minPlaybackInterval);
            healSlot.Init(dealtHeal, counterAudio, minPlaybackInterval);
            nicknameText.text = nickname;
            ViewID = viewID;
        }

        public void PlayTween()
        {
            for (int i = 0; i < tweens.Length; i++)
            {
                tweens[i].DORewind();
                tweens[i].DOPlay();
            }
        }

        public void EnableReadyCheckbox()
        {
            checkBox.SetActive(true);
        }
    }
}
