using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using EverScord.UI;
using EverScord.Armor;
using EverScord.Character;

using Random = UnityEngine.Random;
using Photon.Pun;

namespace EverScord.Augment
{
    public class AugmentPresenter : MonoBehaviour
    {
        private enum AugmentType
        {
            Helmet,
            Vest,
            Shoes
        };

        private const string DOTWEEN_UI_APPEAR  = "AugmentCard_Appear";
        private const string DOTWEEN_UI_DISAPPEAR = "AugmentCard_Disappear";

        private static int selectedPeople = 0;

        [SerializeField] private GameObject uiHub;
        [SerializeField] private SelectUI helmetSelectUI, vestSelectUI, shoesSelectUI;
        [SerializeField] private UpgradeUI helmetUpgradeUI, vestUpgradeUI, shoesUpgradeUI;
        [SerializeField] private GameTimer augmentTimer;
        [SerializeField] private LockableButton confirmBtn;
        [SerializeField] private Button upgradeBtn;
        [SerializeField] private float selectTimeLimit;
        [SerializeField] private ArmorUpgradeIcon helmetUpgradeIcons, vestUpgradeIcons, shoeUpgradeIcons;
        [SerializeField] private List<Sprite> helmetIcons, vestIcons, shoeIcons;
        [SerializeField] private Color greenIconColor, deafultIconColor;

        private List<string> helmetAugmentTags = new();
        private List<string> vestAugmentTags = new();
        private List<string> shoesAugmentTags = new();
        private AugmentData augmentData = new();
        private CharacterControl player;

        private string selectedHelmetTag = "";
        private string selectedVestTag = "";
        private string selectedShoesTag = "";
        private int enhanceIndex = 0;
        private int enhanceCount = 0;
        private bool isAugmentSelectMode => enhanceIndex == 0;

        void Awake()
        {
            augmentData.Init();
            uiHub.SetActive(false);

            GameManager.Instance.InitControl(this);
        }

        void Start()
        {
            player = CharacterControl.CurrentClientCharacter;
        }

        private void RemoveListeners()
        {
            RemoveSlotSelectEvent();
            confirmBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            upgradeBtn.onClick.RemoveAllListeners();
        }

        public void ShowAugmentCards()
        {
            selectedPeople = 0;
            RemoveListeners();

            uiHub.SetActive(true);
            augmentTimer.gameObject.SetActive(true);

            player.PlayerUIControl.SetCursor(CursorType.AUGMENT, 0, 0);
            player.SetState(SetCharState.ADD, CharState.SELECTING_AUGMENT);

            if (PhotonNetwork.IsConnected)
                player.CharacterPhotonView.RPC(nameof(player.SyncState), RpcTarget.Others, player.State);

            if (isAugmentSelectMode)
            {
                SetSelectUI(true);
                SetUpgradeUI(false);

                confirmBtn.ResetState();
                confirmBtn.gameObject.SetActive(true);

                confirmBtn.GetComponent<Button>().onClick.AddListener(EnhanceArmor);
                confirmBtn.GetComponent<Button>().onClick.AddListener(HideAugmentCards);

                augmentTimer.SetTimer(selectTimeLimit, ProceedRandomEnhance);
                augmentTimer.StartTimer();

                helmetSelectUI.Init(TryUnlockConfirmBtn);
                vestSelectUI.Init(TryUnlockConfirmBtn);
                shoesSelectUI.Init(TryUnlockConfirmBtn);

                CreateAugmentSelectTags();
            }
            else
            {
                upgradeBtn.onClick.AddListener(EnhanceArmor);
                upgradeBtn.onClick.AddListener(HideAugmentCards);

                SetSelectUI(false);
                SetUpgradeUI(true);

                augmentTimer.SetTimer(selectTimeLimit, ProceedUpgrade);
                augmentTimer.StartTimer();

                SetAugmentUpgradeText();
            }

            DOTween.Rewind(DOTWEEN_UI_APPEAR);
            DOTween.Play(DOTWEEN_UI_APPEAR);
        }

        private void HideAugmentCards()
        {
            RemoveListeners();

            augmentTimer.gameObject.SetActive(false);
            confirmBtn.gameObject.SetActive(false);
            upgradeBtn.gameObject.SetActive(false);

            DOTween.Rewind(DOTWEEN_UI_DISAPPEAR);
            DOTween.Play(DOTWEEN_UI_DISAPPEAR);

            player.PlayerUIControl.SetCursor(CursorType.BATTLE);
            player.SetState(SetCharState.REMOVE, CharState.SELECTING_AUGMENT);

            if (PhotonNetwork.IsConnected)
            {
                player.CharacterPhotonView.RPC(nameof(player.SyncState), RpcTarget.Others, player.State);
                player.CharacterPhotonView.RPC(nameof(player.SyncOnAugmentSelect), RpcTarget.MasterClient);
            }
        }

        public static void IncreaseSelectedPeople()
        {
            ++selectedPeople;

            if (selectedPeople != PhotonNetwork.CurrentRoom.PlayerCount)
                return;

            selectedPeople = 0;

            if (!PhotonNetwork.IsConnected)
                return;

            PortalControl portal = GameManager.Instance.LevelController.PortalController;
            portal.View.RPC(nameof(portal.SyncSetPortal), RpcTarget.All, true);
        }

        private void CreateAugmentSelectTags()
        {
            helmetAugmentTags.Clear();
            vestAugmentTags.Clear();
            shoesAugmentTags.Clear();

            SetAugmentSelectTags(AugmentType.Helmet);
            SetAugmentSelectTags(AugmentType.Vest);
            SetAugmentSelectTags(AugmentType.Shoes);
        }

        void SetAugmentSelectTags(AugmentType type)
        {
            IDictionary<string, List<ArmorAugment>> augmentDict = null;
            List<string> augmentTags = null;
            SelectUI targetUI = null;

            switch (type)
            {
                case AugmentType.Helmet:
                    // Check dealer or healer
                    augmentDict = augmentData.OffenseHelmetAugmentDict;

                    augmentTags = helmetAugmentTags;
                    targetUI    = helmetSelectUI;
                    break;

                case AugmentType.Vest:
                    augmentDict = augmentData.VestAugmentDict;
                    augmentTags = vestAugmentTags;
                    targetUI    = vestSelectUI;
                    break;

                case AugmentType.Shoes:
                    augmentDict = augmentData.ShoesAugmentDict;
                    augmentTags = shoesAugmentTags;
                    targetUI    = shoesSelectUI;
                    break;

                default:
                    break;
            }

            if (augmentDict == null)
            {
                Debug.LogWarning("Failed to initialize augment dictionary.");
                return;
            }

            int index = 0;

            foreach (KeyValuePair<string, List<ArmorAugment>> record in augmentDict)
            {
                if (index >= targetUI.slotImages.Length)
                    break;

                if (enhanceIndex >= record.Value.Count)
                {
                    Debug.LogWarning($"Enhanced augment does not exist. Current enhance count : {enhanceIndex}");
                    break;
                }

                augmentTags.Add(record.Key);
                targetUI.SetSlotText(index, record.Value[enhanceIndex]?.Description);
                index++;
            }
        }

        private void SetAugmentUpgradeText()
        {
            // Check dealer or healer
            var helmetAugmentDict = augmentData.OffenseHelmetAugmentDict;

            helmetUpgradeUI.SetText(helmetAugmentDict[selectedHelmetTag][enhanceIndex]?.Description);
            vestUpgradeUI.SetText(augmentData.VestAugmentDict[selectedVestTag][enhanceIndex]?.Description);
            shoesUpgradeUI.SetText(augmentData.ShoesAugmentDict[selectedShoesTag][enhanceIndex]?.Description);
        }

        private void TryUnlockConfirmBtn()
        {
            if (helmetSelectUI.selectedSlotIndex == -1)
                return;
            
            if (vestSelectUI.selectedSlotIndex == -1)
                return;

            if (shoesSelectUI.selectedSlotIndex == -1)
                return;

            confirmBtn.UnlockButton();
            RemoveSlotSelectEvent();
        }

        private void ProceedRandomEnhance()
        {
            if (helmetSelectUI.selectedSlotIndex < 0)
            {
                int index = Random.Range(0, helmetAugmentTags.Count);
                helmetSelectUI.SetSlotIndex(index);
            }

            if (vestSelectUI.selectedSlotIndex < 0)
            {
                int index = Random.Range(0, vestAugmentTags.Count);
                vestSelectUI.SetSlotIndex(index);
            }

            if (shoesSelectUI.selectedSlotIndex < 0)
            {
                int index = Random.Range(0, shoesAugmentTags.Count);
                shoesSelectUI.SetSlotIndex(index);
            }

            confirmBtn.GetComponent<Button>().onClick?.Invoke();
        }

        private void ProceedUpgrade()
        {
            upgradeBtn.GetComponent<Button>().onClick?.Invoke();
        }

        private void EnhanceArmor()
        {
            // Check dealer or healer
            var helmetAugmentDict       = augmentData.OffenseHelmetAugmentDict;

            if (isAugmentSelectMode)
            {
                selectedHelmetTag       = helmetAugmentTags[helmetSelectUI.selectedSlotIndex];
                selectedVestTag         = vestAugmentTags[vestSelectUI.selectedSlotIndex];
                selectedShoesTag        = shoesAugmentTags[shoesSelectUI.selectedSlotIndex];
            }

            HelmetAugment helmetAugment = (HelmetAugment)helmetAugmentDict[selectedHelmetTag][enhanceIndex];
            VestAugment vestAugment     = (VestAugment)augmentData.VestAugmentDict[selectedVestTag][enhanceIndex];
            ShoesAugment shoesAugment   = (ShoesAugment)augmentData.ShoesAugmentDict[selectedShoesTag][enhanceIndex];

            player.SetArmor(new HelmetDecorator(player.CharacterHelmet, helmetAugment));
            player.SetArmor(new VestDecorator(player.CharacterVest, vestAugment));
            player.SetArmor(new ShoesDecorator(player.CharacterShoes, shoesAugment));

            enhanceIndex++;
            enhanceCount++;

            if (enhanceIndex >= helmetAugmentDict[selectedHelmetTag].Count)
                enhanceIndex = helmetAugmentDict[selectedHelmetTag].Count - 1;
        }

        private void RemoveSlotSelectEvent()
        {
            helmetSelectUI.RemoveSlotSelectEvent(TryUnlockConfirmBtn);
            vestSelectUI.RemoveSlotSelectEvent(TryUnlockConfirmBtn);
            shoesSelectUI.RemoveSlotSelectEvent(TryUnlockConfirmBtn);
        }

        private void SetUpgradeUI(bool isEnabled)
        {
            upgradeBtn.gameObject.SetActive(isEnabled);
            helmetUpgradeUI.gameObject.SetActive(isEnabled);
            vestUpgradeUI.gameObject.SetActive(isEnabled);
            shoesUpgradeUI.gameObject.SetActive(isEnabled);

            if (!isEnabled || enhanceCount == 0)
                return;

            List<Sprite>[] armorIcons = { helmetIcons, vestIcons, shoeIcons };
            ArmorUpgradeIcon[] upgradeIcons = { helmetUpgradeIcons, vestUpgradeIcons, shoeUpgradeIcons };

            for (int i = 0; i < armorIcons.Length; i++)
            {
                if (armorIcons[i].Count <= 0 || enhanceCount >= armorIcons[i].Count)
                    continue;

                upgradeIcons[i].PreviousIcon.color = deafultIconColor;
                upgradeIcons[i].NextIcon.color = deafultIconColor;

                if (enhanceCount == 1)
                    upgradeIcons[i].PreviousIcon.color = greenIconColor;

                upgradeIcons[i].PreviousIcon.sprite = armorIcons[i][enhanceCount - 1];
                upgradeIcons[i].NextIcon.sprite = armorIcons[i][enhanceCount];
            }
        }

        private void SetSelectUI(bool isEnabled)
        {
            helmetSelectUI.gameObject.SetActive(isEnabled);
            vestSelectUI.gameObject.SetActive(isEnabled);
            shoesSelectUI.gameObject.SetActive(isEnabled);
        }
    }

    [System.Serializable]
    public class ArmorUpgradeIcon
    {
        public Image PreviousIcon;
        public Image NextIcon;
    }
}
