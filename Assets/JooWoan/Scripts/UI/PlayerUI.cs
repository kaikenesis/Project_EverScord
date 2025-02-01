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

        public void SetReloadText()
        {
            DOTween.Rewind(ConstStrings.TWEEN_AMMORELOAD);
            DOTween.Play(ConstStrings.TWEEN_AMMORELOAD);
        }
    }
}
