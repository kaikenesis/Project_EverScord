using UnityEngine;
using TMPro;
using DG.Tweening;
using Photon.Pun;
using EverScord.Character;
using System.Collections;

namespace EverScord.UI
{
    public class PlayerUI : MonoBehaviour
    {
        private const string BLOOD_ENABLED = "_enabled";
        private const string BLOOD_SIZE = "_Mask_Size";
        private const float BLOOD_THRESHOLD = 0.5f;
        private const float BLOOD_SPEED = 1f;
        public RectTransform CanvasRect { get; private set; }

        [SerializeField] private TextMeshProUGUI currentAmmoText, maxAmmoText;
        [SerializeField] private Color32 initialAmmoTextColor, outOfAmmoTextColor;
        private static Texture2D cursorIcon;
        private static Material bloodMat;
        private Coroutine bloodCoroutine;
        private float maskSize = 1f;
        private bool isEnabled = false;

        void Awake()
        {
            cursorIcon = ResourceManager.Instance.GetAsset<Texture2D>(ConstStrings.KEY_CROSSHAIR);
            bloodMat = ResourceManager.Instance.GetAsset<Material>(ConstStrings.KEY_BLOOD_MAT);

            CanvasRect = transform.parent.GetComponent<RectTransform>();
            Vector2 cursorCenter = new Vector2(cursorIcon.width * 0.5f, cursorIcon.height * 0.5f);
            Cursor.SetCursor(cursorIcon, cursorCenter, CursorMode.Auto);
        }

        void OnDisable()
        {
            bloodMat.SetFloat(BLOOD_SIZE, 1f);
            bloodMat.SetInt(BLOOD_ENABLED, 0);
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

        public void SetBloodyScreen(float currentHealth, float maxHealth)
        {
            SetBloodyScreen(currentHealth <= maxHealth * 0.1f);
        }

        private void SetBloodyScreen(bool isEnabled)
        {
            if (this.isEnabled == isEnabled)
                return;
            
            this.isEnabled = isEnabled;

            if (bloodCoroutine != null)
                StopCoroutine(bloodCoroutine);
            
            bloodCoroutine = StartCoroutine(AnimateBloodyScreen());
        }

        private IEnumerator AnimateBloodyScreen()
        {
            maskSize = 1f;
            float goal = BLOOD_THRESHOLD;
            float speed = -BLOOD_SPEED;

            if (!isEnabled)
            {
                maskSize = BLOOD_THRESHOLD;
                goal = 1f;
                speed *= -1;
            }

            if (isEnabled)
                bloodMat.SetInt(BLOOD_ENABLED, 1);

            while (maskSize <= 1 && maskSize >= BLOOD_THRESHOLD)
            {
                maskSize += speed * Time.deltaTime;
                bloodMat.SetFloat(BLOOD_SIZE, maskSize);

                yield return null;
            }

            maskSize = goal;
            bloodMat.SetFloat(BLOOD_SIZE, goal);

            if (!isEnabled)
                bloodMat.SetInt(BLOOD_ENABLED, 0);

            bloodCoroutine = null;
        }
    }
}
