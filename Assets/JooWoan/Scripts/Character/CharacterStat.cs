using System;
using System.Collections.Generic;
using UnityEngine;
using EverScord.Augment;
using Photon.Pun;

namespace EverScord.Character
{
    public class CharacterStat : MonoBehaviour
    {
        private const float SPEED_FACTOR = 0.01f * 2f;
        [SerializeField] private float currentHealth;

        public static Action<float> OnHealthUpdated = delegate { };
        public Action OnStatEnhanced = delegate { };

        public IDictionary<StatType, StatBonus> BonusDict => bonusDict;
        public bool IsInitialized => bonusDict != null;

        private CharacterControl character;
        private IDictionary<StatType, StatBonus> bonusDict;
        private float maxHealth, moveSpeed, attack, critChance, defense, healthRegen;

        void Awake()
        {
            OnStatEnhanced -= UpdateSkillTimers;
            OnStatEnhanced += UpdateSkillTimers;
        }

        void OnDestroy()
        {
            OnStatEnhanced -= UpdateSkillTimers;
        }

        public void InitBaseStat(CharacterControl character)
        {
            if (IsInitialized)
                return;

            bonusDict = new Dictionary<StatType, StatBonus>();

            int statCount = (int)StatType.END;

            for (int i = 0; i < statCount; i++)
                bonusDict[(StatType)i] = StatBonus.GetDefaultBonus();

            SetAlterationBonus();

            this.character = character;
            string tag = PlayerData.GetCharacterName(character.CharacterType);
            StatInfo info = StatData.StatInfoDict[tag];

            maxHealth       = info.health;
            healthRegen     = info.healthRegen;
            attack          = info.attack;
            defense         = info.defense;
            critChance      = info.critChance;
            moveSpeed       = info.speed * SPEED_FACTOR;

            currentHealth   = MaxHealth;

            if (character.CharacterJob == PlayerData.EJob.Healer)
                attack = info.supportAttack;
        }

        public void SetAlterationBonus()
        {
            // statValues are aligned in the order of StatType enum
            float[] statValues = GameManager.Instance.PlayerAlterationData.alterationStatus.statusValues;

            for (int i = 0; i < statValues.Length; i++)
                bonusDict[(StatType)i] = StatBonus.CreateBonus(0, statValues[i]);
        }

        public float Attack
        {
            get
            {
                StatBonus bonus1 = character.CharacterHelmet.BasicAttackBonus;

                if (character.CharacterJob == PlayerData.EJob.Dealer)
                {
                    StatBonus merged1 = StatBonus.MergeBonus(bonus1, bonusDict[StatType.ATTACK]);
                    return merged1.CalculateStat(attack);
                }

                StatBonus bonus2  = character.CharacterHelmet.AllroundHealBonus;
                StatBonus bonus3  = StatBonus.MergeBonus(bonus1, bonus2);
                StatBonus merged2 = StatBonus.MergeBonus(bonus3, bonusDict[StatType.HEAL_INCREASE]);

                return merged2.CalculateStat(attack);
            }
        }

        public float MaxHealth
        {
            get
            {
                StatBonus bonus1 = character.CharacterVest.HealthBonus;
                StatBonus merged = StatBonus.MergeBonus(bonus1, bonusDict[StatType.MAXHEALTH]);

                return merged.CalculateStat(maxHealth);
            }
        }

        public float HealthRegen
        {
            get
            {
                StatBonus bonus1 = character.CharacterVest.HealthRegenerationBonus;
                StatBonus merged = StatBonus.MergeBonus(bonus1, bonusDict[StatType.HEALTH_REGEN]);

                return merged.CalculateStat(healthRegen);
            }
        }

        public float Defense
        {
            get
            {
                StatBonus bonus1 = character.CharacterVest.DefenseBonus;
                StatBonus merged = StatBonus.MergeBonus(bonus1, bonusDict[StatType.DEFENSE]);

                return merged.CalculateStat(defense);
            }
        }

        public float Speed
        {
            get
            {
                StatBonus bonus1 = character.CharacterShoes.MoveSpeedBonus;
                StatBonus merged = StatBonus.MergeBonus(bonus1, bonusDict[StatType.SPEED]);

                return merged.CalculateStat(moveSpeed);
            }
        }

        public float DecreasedCooldown(float cooldown)
        {
            StatBonus bonus1 = character.CharacterShoes.CooldownBonus;
            StatBonus merged = StatBonus.MergeBonus(bonus1, bonusDict[StatType.COOLDOWN_DECREASE]);

            return merged.CalculateStat(cooldown);
        }

        public float DecreasedReload(float duration)
        {
            StatBonus bonus1 = character.CharacterShoes.ReloadSpeedBonus;
            StatBonus merged = StatBonus.MergeBonus(bonus1, bonusDict[StatType.RELOADSPEED_DECREASE]);

            return merged.CalculateStat(duration);
        }

        public float IncreasedSkillDamage(float damage)
        {
            StatBonus bonus1 = character.CharacterHelmet.SkillAttackBonus;

            if (character.CharacterJob == PlayerData.EJob.Dealer)
            {
                StatBonus merged1 = StatBonus.MergeBonus(bonus1, bonusDict[StatType.SKILLDAMAGE_INCREASE]);
                return merged1.CalculateStat(damage);
            }

            StatBonus bonus2  = character.CharacterHelmet.AllroundHealBonus;
            StatBonus bonus3  = StatBonus.MergeBonus(bonus1, bonus2);
            StatBonus merged2 = StatBonus.MergeBonus(bonus3, bonusDict[StatType.HEAL_INCREASE]);
            
            return merged2.CalculateStat(damage);
        }

        public float CriticalHitChance
        {
            get { return critChance; }
        }

        public float CurrentHealth
        {
            get { return currentHealth; }
            set
            {
                bool previousIsLowHealth = character.IsLowHealth;

                currentHealth = value;

                bool afterIsLowHealth = character.IsLowHealth;

                if (!character.IsDead && currentHealth <= 0)
                {
                    character.StartDeath();
                    character.BlinkEffects.StopAllBlinks();
                }
                else if (previousIsLowHealth != afterIsLowHealth)
                    character.BlinkEffects.LoopBlink(character.IsLowHealth);

                if (character.CharacterPhotonView.IsMine)
                {
                    character.PlayerUIControl.SetGrayscaleScreen(currentHealth);
                    character.PlayerUIControl.SetBloodyScreen(currentHealth, afterIsLowHealth);
                    OnHealthUpdated?.Invoke(currentHealth / Mathf.Max(0.01f, MaxHealth));
                }
            }
        }

        public void SetStat(StatType type, float value)
        {
            ref float targetStat = ref maxHealth;

            switch (type)
            {
                case StatType.MAXHEALTH:
                    targetStat = ref maxHealth;
                    break;

                case StatType.SPEED:
                    targetStat = ref moveSpeed;
                    break;

                case StatType.ATTACK:
                    targetStat = ref attack;
                    break;

                case StatType.DEFENSE:
                    targetStat = ref defense;
                    break;

                case StatType.HEALTH_REGEN:
                    targetStat = ref healthRegen;
                    break;

                case StatType.CRITICAL_CHANCE:
                    targetStat = ref critChance;
                    break;

                default:
                    return;
            }

            targetStat = value;
        }

        // Sets bonus which modifies stat by percentage
        // ex) Pass in 30f for 30% increase, -50f for 50% decrease
        public void SetAlterationBonus(StatType type, float additive, float multiplicative)
        {
            if (type == StatType.END)
                return;
            
            bonusDict[type] = StatBonus.CreateBonus(additive, multiplicative);
            OnStatEnhanced?.Invoke();

            if (PhotonNetwork.IsConnected && character.CharacterPhotonView.IsMine)
                character.CharacterPhotonView.RPC(nameof(character.SyncAlterationBonus), RpcTarget.Others, (int)type, additive, multiplicative);
        }

        private void UpdateSkillTimers()
        {
            for (int i = 0; i < character.SkillList.Count; i++)
                character.SkillList[i].SkillAction.SetNewCooldown();
        }
    }
    
    public enum StatType
    {
        ATTACK,
        DEFENSE,
        CRITICAL_CHANCE,
        SKILLDAMAGE_INCREASE,
        HEAL_INCREASE,
        MAXHEALTH,
        HEALTH_REGEN,
        SPEED,
        COOLDOWN_DECREASE,
        RELOADSPEED_DECREASE,
        END
    }
}
