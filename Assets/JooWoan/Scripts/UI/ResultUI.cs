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

        private DOTweenAnimation[] tweens;

        void Awake()
        {
            tweens = GetComponentsInChildren<DOTweenAnimation>();
        }

        public void Init(int killCount, float dealtDamage, float dealtHeal, string nickname)
        {
            killSlot.Init(killCount);
            damageSlot.Init(dealtDamage);
            healSlot.Init(dealtHeal);
            nicknameText.text = nickname;
        }

        public void PlayTween()
        {
            for (int i = 0; i < tweens.Length; i++)
            {
                tweens[i].DORewind();
                tweens[i].DOPlay();
            }
        }
    }
}
