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
        [SerializeField] private TextMeshProUGUI usernameText;

        void OnEnable()
        {
            DOTweenAnimation[] tweens = usernameText.GetComponentsInChildren<DOTweenAnimation>();

            for (int i = 0; i < tweens.Length; i++)
                tweens[i].DORewind();
        }

        public void Init(int killCount, float dealtDamage, float dealtHeal)
        {
            killSlot.Init(killCount);
            damageSlot.Init(dealtDamage);
            healSlot.Init(dealtHeal);
        }
    }
}
