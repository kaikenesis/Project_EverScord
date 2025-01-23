using EverScord.Character;
using UnityEngine;
using TMPro;

namespace EverScord.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [field: SerializeField] public RectTransform CursorRect { get; private set; }
        public CustomCursor PlayerCursor                        { get; private set; }
        public RectTransform CanvasRect                         { get; private set; }

        [SerializeField] private TextMeshProUGUI currentAmmoText, maxAmmoText;

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
            currentAmmoText.text = $"{count}";
        }
    }
}
