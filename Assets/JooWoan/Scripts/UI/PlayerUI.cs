using UnityEngine;
using TMPro;
using DG.Tweening;
using Photon.Pun;
using EverScord.Character;

namespace EverScord.UI
{
    public class PlayerUI : MonoBehaviour
    {
        public RectTransform CanvasRect { get; private set; }

        [SerializeField] private TextMeshProUGUI currentAmmoText, maxAmmoText;
        [SerializeField] private Color32 initialAmmoTextColor, outOfAmmoTextColor;
        [SerializeField] private Texture2D cursorIcon;

        void Awake()
        {
            CanvasRect = transform.parent.GetComponent<RectTransform>();
            Vector2 cursorCenter = new Vector2(cursorIcon.width * 0.5f, cursorIcon.height * 0.5f);
            Cursor.SetCursor(cursorIcon, cursorCenter, CursorMode.Auto);
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

        public void SetAimCursor(CharacterControl character, bool state)
        {
            if (PhotonNetwork.IsConnected && !character.CharacterPhotonView.IsMine)
                return;
            
            Cursor.visible = state;
        }
    }
}
