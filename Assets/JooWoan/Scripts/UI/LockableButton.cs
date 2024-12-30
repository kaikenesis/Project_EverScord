using UnityEngine;
using UnityEngine.UI;

namespace EverScord.UI
{
    public class LockableButton : MonoBehaviour
    {
        [SerializeField] private bool isLocked;
        [SerializeField] private float lockAlpha;
        private Button button;
        private Image buttonImg;

        void Awake()
        {
            button = GetComponent<Button>();
            buttonImg = GetComponent<Image>();
        }

        void Start()
        {
            if (isLocked)
                LockButton();
        }

        public void LockButton()
        {
            button.enabled = false;
            Color32 buttonColor = buttonImg.color;
            buttonImg.color = new Color32(buttonColor.r, buttonColor.g, buttonColor.b, (byte)lockAlpha);
        }

        public void UnlockButton()
        {
            button.enabled = true;
            Color32 buttonColor = buttonImg.color;
            buttonImg.color = new Color32(buttonColor.r, buttonColor.g, buttonColor.b, 255);
        }
    }
}
