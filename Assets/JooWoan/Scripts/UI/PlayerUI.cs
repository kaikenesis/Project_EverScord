using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using EverScord.Character;
using EverScord.Weapons;

namespace EverScord.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [field: SerializeField] public RectTransform CursorRect { get; private set; }
        public CustomCursor PlayerCursor                        { get; private set; }
        public RectTransform CanvasRect                         { get; private set; }

        [SerializeField] private TextMeshProUGUI currentAmmoText, maxAmmoText;
        [SerializeField] private Color32 initialAmmoTextColor, outOfAmmoTextColor;

        public void Init(CharacterControl player)
        {
            CanvasRect = transform.parent.GetComponent<RectTransform>();
            PlayerCursor = new CustomCursor(player);
        }

        void Update()
        {
            if (PlayerCursor == null)
                return;

            PlayerCursor.SetCursorPosition(this);
        }

        public void SetAmmoText(int count)
        {
            DOTween.Rewind(ConstStrings.TWEEN_AMMOCHANGE);
            DOTween.Play(ConstStrings.TWEEN_AMMOCHANGE);
            
            currentAmmoText.text = $"{count}";
            currentAmmoText.color = count <= 1 ? outOfAmmoTextColor : initialAmmoTextColor;
        }

        public IEnumerator RollAmmoText(Weapon weapon)
        {
            int count = 0;
            float interval = weapon.ReloadTime / weapon.MaxAmmo;

            DOTween.Rewind("ReloadAmmo");
            DOTween.Play("ReloadAmmo");

            for (int i = 0; count <= weapon.MaxAmmo; i++)
            {
                currentAmmoText.text = $"{count++}";
                yield return new WaitForSeconds(interval);
            }

            DOTween.Rewind("ReloadAmmo");
        }
    }
}
