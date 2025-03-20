using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;
using EverScord.UI;
using EverScord.Armor;
using EverScord.Character;

using Random = UnityEngine.Random;
using ArmorType = EverScord.ArmorData.Armor.EType;

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

        private List<string> helmetAugmentTags = new();
        private List<string> vestAugmentTags = new();
        private List<string> shoesAugmentTags = new();
        private AugmentData augmentData = new();
        private CharacterControl player;

        private string selectedHelmetTag = "";
        private string selectedVestTag = "";
        private string selectedShoesTag = "";
        private float helmetStatIncrease, vestStatIncrease, shoesStatIncrease;
        private float previousHelmetStatIncrease, previousVestStatIncrease, previousShoesStatIncrease;
        private int enhanceIndex = 0;
        private int enhanceCount = 0;

        private Action onEnhanced;

        private bool isAugmentSelectMode => enhanceIndex == 0;

        void Awake()
        {
            augmentData.Init();
            uiHub.SetActive(false);

            enhanceIndex = 0;
            enhanceCount = 0;

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

        public void SubscribeOnEnhanced(Action subscriber)
        {
            onEnhanced -= subscriber;
            onEnhanced += subscriber;
        }

        public void UnsubscribeOnEnhanced(Action subscriber)
        {
            onEnhanced -= subscriber;
        }

        public void ShowAugmentCards()
        {
            selectedPeople = 0;
            RemoveListeners();

            uiHub.SetActive(true);
            augmentTimer.gameObject.SetActive(true);

            PlayerUI.SetCursor(CursorType.UIFOCUS, 0, 0);
            player.SetState(SetCharState.ADD, CharState.INTERACTING_UI);

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

                SetStatIncrease();
                SetAugmentUpgradeText();
            }
            SoundManager.Instance.PlaySound("ClearWinSound");
            DOTween.Rewind(DOTWEEN_UI_APPEAR);
            DOTween.Play(DOTWEEN_UI_APPEAR);
        }

        private void SetStatIncrease()
        {
            var helmetAugmentDict = augmentData.OffenseHelmetAugmentDict;

            if (player.CharacterJob == PlayerData.EJob.Healer)
                helmetAugmentDict = augmentData.SupportHelmetAugmentDict;
            
            HelmetAugment helmetAugment = (HelmetAugment)helmetAugmentDict[selectedHelmetTag][enhanceIndex];
            VestAugment vestAugment     = (VestAugment)augmentData.VestAugmentDict[selectedVestTag][enhanceIndex];
            ShoesAugment shoesAugment   = (ShoesAugment)augmentData.ShoesAugmentDict[selectedShoesTag][enhanceIndex];

            IHelmet originalHelmet  = player.CharacterHelmet;
            IVest originalVest      = player.CharacterVest;
            IShoes originalShoes    = player.CharacterShoes;

            if (enhanceIndex > 0)
            {
                originalHelmet = ((HelmetDecorator)player.CharacterHelmet).originalHelmet;
                originalVest   = ((VestDecorator)player.CharacterVest).originalVest;
                originalShoes  = ((ShoesDecorator)player.CharacterShoes).originalShoes;
            }

            previousHelmetStatIncrease  = helmetStatIncrease;
            previousVestStatIncrease    = vestStatIncrease;
            previousShoesStatIncrease   = shoesStatIncrease;

            helmetStatIncrease  = IArmor.GetStatChangeAmount(originalHelmet, new HelmetDecorator(player.CharacterHelmet, helmetAugment), helmetAugment.BonusIndex);
            vestStatIncrease    = IArmor.GetStatChangeAmount(originalVest,   new VestDecorator(player.CharacterVest,     vestAugment),   vestAugment.BonusIndex);
            shoesStatIncrease   = IArmor.GetStatChangeAmount(originalShoes,  new ShoesDecorator(player.CharacterShoes,   shoesAugment),  shoesAugment.BonusIndex);
        }

        private void HideAugmentCards()
        {
            RemoveListeners();

            augmentTimer.gameObject.SetActive(false);
            confirmBtn.gameObject.SetActive(false);
            upgradeBtn.gameObject.SetActive(false);

            DOTween.Rewind(DOTWEEN_UI_DISAPPEAR);
            DOTween.Play(DOTWEEN_UI_DISAPPEAR);

            PlayerUI.SetCursor(CursorType.BATTLE);
            player.SetState(SetCharState.REMOVE, CharState.INTERACTING_UI);

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
                    augmentDict = augmentData.OffenseHelmetAugmentDict;

                    if (player.CharacterJob == PlayerData.EJob.Healer)
                        augmentDict = augmentData.SupportHelmetAugmentDict;

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
                targetUI.SetSlotText(index, record.Value[enhanceIndex]);
                index++;
            }
        }

        private void SetAugmentUpgradeText()
        {
            var helmetAugmentDict = augmentData.OffenseHelmetAugmentDict;

            if (player.CharacterJob == PlayerData.EJob.Healer)
                helmetAugmentDict = augmentData.SupportHelmetAugmentDict;

            helmetUpgradeUI.SetText(helmetAugmentDict[selectedHelmetTag][enhanceIndex], previousHelmetStatIncrease, helmetStatIncrease);
            vestUpgradeUI.SetText(augmentData.VestAugmentDict[selectedVestTag][enhanceIndex], previousVestStatIncrease, vestStatIncrease);
            shoesUpgradeUI.SetText(augmentData.ShoesAugmentDict[selectedShoesTag][enhanceIndex], previousShoesStatIncrease, shoesStatIncrease);
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
            var helmetAugmentDict = augmentData.OffenseHelmetAugmentDict;

            if (player.CharacterJob == PlayerData.EJob.Healer)
                helmetAugmentDict = augmentData.SupportHelmetAugmentDict;

            if (isAugmentSelectMode)
            {
                selectedHelmetTag       = helmetAugmentTags[helmetSelectUI.selectedSlotIndex];
                selectedVestTag         = vestAugmentTags[vestSelectUI.selectedSlotIndex];
                selectedShoesTag        = shoesAugmentTags[shoesSelectUI.selectedSlotIndex];

                helmetStatIncrease      = helmetAugmentDict[selectedHelmetTag][enhanceIndex].DescriptionValue;
                vestStatIncrease        = augmentData.VestAugmentDict[selectedVestTag][enhanceIndex].DescriptionValue;
                shoesStatIncrease       = augmentData.ShoesAugmentDict[selectedShoesTag][enhanceIndex].DescriptionValue;
            }

            HelmetAugment helmetAugment = (HelmetAugment)helmetAugmentDict[selectedHelmetTag][enhanceIndex];
            VestAugment vestAugment     = (VestAugment)augmentData.VestAugmentDict[selectedVestTag][enhanceIndex];
            ShoesAugment shoesAugment   = (ShoesAugment)augmentData.ShoesAugmentDict[selectedShoesTag][enhanceIndex];

            player.SetArmor(new HelmetDecorator(player.CharacterHelmet, helmetAugment));
            player.SetArmor(new VestDecorator(player.CharacterVest, vestAugment));
            player.SetArmor(new ShoesDecorator(player.CharacterShoes, shoesAugment));
            player.Stats.OnStatEnhanced?.Invoke();

            if (PhotonNetwork.IsConnected)
                player.CharacterPhotonView.RPC(nameof(player.SyncArmor), RpcTarget.Others, selectedHelmetTag, selectedVestTag, selectedShoesTag, enhanceIndex);

            enhanceIndex++;
            enhanceCount++;
            onEnhanced?.Invoke();

            if (enhanceIndex >= helmetAugmentDict[selectedHelmetTag].Count)
                enhanceIndex = helmetAugmentDict[selectedHelmetTag].Count - 1;

            SoundManager.Instance.PlaySound("ButtonSound");
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

            ArmorType[] armorTypes = { ArmorType.Head, ArmorType.Chest, ArmorType.Foot };
            ArmorUpgradeIcon[] upgradeIcons = { helmetUpgradeIcons, vestUpgradeIcons, shoeUpgradeIcons };

            for (int i = 0; i < armorTypes.Length; i++)
            {
                ArmorType type = (ArmorType)i;
                upgradeIcons[i].PreviousIcon.sprite = GetArmorIcon(type, enhanceCount);
                upgradeIcons[i].NextIcon.sprite     = GetArmorIcon(type, enhanceCount + 1);
            }
        }

        private void SetSelectUI(bool isEnabled)
        {
            helmetSelectUI.gameObject.SetActive(isEnabled);
            vestSelectUI.gameObject.SetActive(isEnabled);
            shoesSelectUI.gameObject.SetActive(isEnabled);
        }

        public static Sprite GetArmorIcon(ArmorType type, int curLevel)
        {
            ArmorData.Armor[] armors = GameManager.Instance.ArmorData.Armors;

            for (int i = 0; i < armors.Length; i++)
            {
                if (type == armors[i].ArmorType)
                    return armors[i].SourceImg[curLevel];
            }

            return null;
        }

        public void SyncPlayerArmor(CharacterControl target, string helmetTag, string vestTag, string shoesTag, int index)
        {
            var helmetAugmentDict = augmentData.OffenseHelmetAugmentDict;

            if (target.CharacterJob == PlayerData.EJob.Healer)
                helmetAugmentDict = augmentData.SupportHelmetAugmentDict;

            HelmetAugment helmetAugment = (HelmetAugment)helmetAugmentDict[helmetTag][index];
            VestAugment vestAugment = (VestAugment)augmentData.VestAugmentDict[vestTag][index];
            ShoesAugment shoesAugment = (ShoesAugment)augmentData.ShoesAugmentDict[shoesTag][index];

            target.SetArmor(new HelmetDecorator(target.CharacterHelmet, helmetAugment));
            target.SetArmor(new VestDecorator(target.CharacterVest, vestAugment));
            target.SetArmor(new ShoesDecorator(target.CharacterShoes, shoesAugment));
            target.Stats.OnStatEnhanced?.Invoke();
        }
    }

    [System.Serializable]
    public class ArmorUpgradeIcon
    {
        public Image PreviousIcon;
        public Image NextIcon;
    }
}
