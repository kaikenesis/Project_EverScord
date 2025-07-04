using System.Collections;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using DG.Tweening;
using Photon.Pun;
using EverScord.Character;
using EverScord.GameCamera;
using EverScord.Effects;

namespace EverScord.UI
{
    public class PlayerUI : MonoBehaviour
    {
        private const string BLOOD_ENABLED = "_enabled";
        private const string BLOOD_SIZE = "_Mask_Size";
        private const float BLOOD_THRESHOLD = 0.5f;
        private const float BLOOD_SPEED = 1f;

        public static Transform Root
        {
            get
            {
                if (root)
                    return root;
                
                return root = GameObject.FindGameObjectWithTag(ConstStrings.TAG_UIROOT).transform;
            }
        }
        
        private static Transform root;
        private static Texture2D cursorIcon;
        private static Material bloodMat;
        private static ColorCurves colorCurves;

        [SerializeField] private GameObject notificationBox;
        [SerializeField] private TextMeshProUGUI currentAmmoText, maxAmmoText, countdownText;
        [SerializeField] private Color32 initialAmmoTextColor, outOfAmmoTextColor;
        [SerializeField] private Transform iconTransform;

        private Coroutine bloodCoroutine;
        private float maskSize = 1f;
        private bool isEnabled = false;

        public void Init(GameObject iconPrefab)
        {
            if (!bloodMat) 
                bloodMat = ResourceManager.Instance.GetAsset<Material>(AssetReferenceManager.BloodMat_ID);
            
            var icon = Instantiate(iconPrefab, iconTransform.transform.position, Quaternion.identity);
            icon.transform.SetParent(transform);
            SetCursor(CursorType.BATTLE);

            Volume volume = CharacterCamera.Root.GetComponent<Volume>();
            VolumeProfile profile = volume.profile;

            if (profile.TryGet<ColorCurves>(out var curves))
            {
                colorCurves = curves;
            }
        }

        void OnDisable()
        {
            bloodMat.SetFloat(BLOOD_SIZE, 1f);
            bloodMat.SetInt(BLOOD_ENABLED, 0);
            SetGrayscaleScreen(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
                SetGrayscaleScreen(true);
        }

        public static void SetCursor(CursorType type, float xPos = 0.5f, float yPos = 0.5f)
        {
            cursorIcon = GameManager.Instance.CrossHairCursor;

            switch (type)
            {
                case CursorType.UIFOCUS:
                    xPos = 0f; yPos = 0f;
                    cursorIcon = GameManager.Instance.UIFocusCursor;
                    break;
            }

            Vector2 cursorCenter = new Vector2(cursorIcon.width * xPos, cursorIcon.height * yPos);
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

        public void SetBloodyScreen(float currentHealth, bool isLowHealth)
        {
            if (currentHealth <= 0)
            {
                bloodMat.SetInt(BLOOD_ENABLED, 0);
                return;
            }

            if (isEnabled == isLowHealth)
                return;
            
            isEnabled = isLowHealth;

            if (gameObject.activeSelf && bloodCoroutine != null)
                StopCoroutine(bloodCoroutine);
            
            if (gameObject.activeSelf)
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

        public void SetGrayscaleScreen(float currentHealth)
        {
            SetGrayscaleScreen(currentHealth <= 0);
        }

        private void SetGrayscaleScreen(bool state)
        {
            if (colorCurves)
                colorCurves.active = state;
        }

        public void ShowPortalNotification()
        {
            notificationBox.SetActive(true);

            SoundManager.Instance.PlaySound(ConstStrings.SFX_PORTAL_ACTIVATED);

            DOTween.Rewind(ConstStrings.TWEEN_STAGE_NOTIFICATION);
            DOTween.Play(ConstStrings.TWEEN_STAGE_NOTIFICATION);
        }

        public void HidePortalNotification()
        {
            DOTween.Rewind(ConstStrings.TWEEN_STAGE_NOTIFICATION_OFF);
            DOTween.Play(ConstStrings.TWEEN_STAGE_NOTIFICATION_OFF);
        }

        public void ChangePortalCountdownNumber(int countdown)
        {
            countdownText.text = countdown.ToString();

            DOTween.Rewind(ConstStrings.TWEEN_STAGE_COUNTDOWN);
            DOTween.Play(ConstStrings.TWEEN_STAGE_COUNTDOWN);

            SoundManager.Instance.PlaySound(ConstStrings.SFX_PORTAL_TICK);
        }
    }
    public enum CursorType
    {
        BATTLE,
        UIFOCUS
    }
}
