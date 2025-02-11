using UnityEngine;
using TMPro;
using DG.Tweening;
using EverScord.Character;

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
            if (PlayerCursor == null || !CursorRect.gameObject.activeSelf)
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
            currentAmmoText.color = outOfAmmoTextColor;

            DOTween.Rewind(ConstStrings.TWEEN_AMMORELOAD);
            DOTween.Play(ConstStrings.TWEEN_AMMORELOAD);
        }

        public void SetAimCursor(bool state)
        {
            CursorRect.gameObject.SetActive(state);
        }
    }
}
