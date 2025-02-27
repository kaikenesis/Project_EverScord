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

        public static Transform Root { get; private set; }
        public ReviveCircle ReviveCircleControl => reviveCircle;
        private static Texture2D cursorIcon;
        private static Material bloodMat;
        private static ColorCurves colorCurves;

        [SerializeField] private TextMeshProUGUI currentAmmoText, maxAmmoText;
        [SerializeField] private Color32 initialAmmoTextColor, outOfAmmoTextColor;

        private ReviveCircle reviveCircle;
        private Coroutine bloodCoroutine;
        private float maskSize = 1f;
        private bool isEnabled = false;

        public void Init()
        {
            if (!cursorIcon) cursorIcon = ResourceManager.Instance.GetAsset<Texture2D>(AssetReferenceManager.CrosshairIcon_ID);
            if (!bloodMat)   bloodMat   = ResourceManager.Instance.GetAsset<Material>(AssetReferenceManager.BloodMat_ID);

            Vector2 cursorCenter = new Vector2(cursorIcon.width * 0.5f, cursorIcon.height * 0.5f);
            Cursor.SetCursor(cursorIcon, cursorCenter, CursorMode.Auto);

            Volume volume = CharacterCamera.Root.GetComponent<Volume>();
            VolumeProfile profile = volume.sharedProfile;

            if (profile.TryGet<ColorCurves>(out var curves))
                colorCurves = curves;
        }

        void OnDisable()
        {
            bloodMat.SetFloat(BLOOD_SIZE, 1f);
            bloodMat.SetInt(BLOOD_ENABLED, 0);
            SetGrayscaleScreen(false);
        }

        public static void SetUIRoot()
        {
            if (Root == null)
                Root = GameObject.FindGameObjectWithTag(ConstStrings.TAG_UIROOT).transform;
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

        public void SetGrayscaleScreen(float currentHealth)
        {
            SetGrayscaleScreen(currentHealth <= 0);
        }

        private void SetGrayscaleScreen(bool state)
        {
            if (colorCurves)
                colorCurves.active = state;
        }

        public void InitReviveCircle(int viewID)
        {
            Transform uiOwner = GameManager.Instance.PlayerDict[viewID].PlayerTransform;

            var circlePrefab = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.ReviveCircle_ID);
            reviveCircle = Instantiate(circlePrefab, uiOwner).GetComponent<ReviveCircle>();

            reviveCircle.Init(viewID);
            reviveCircle.transform.SetParent(Root.parent);
            reviveCircle.gameObject.SetActive(false);
        }

        public void EnableReviveCircle(bool state)
        {
            Debug.Log($"Whyy {state}");
            reviveCircle.gameObject.SetActive(state);
        }
    }
}
