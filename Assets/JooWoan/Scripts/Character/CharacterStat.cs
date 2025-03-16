using System;
using System.Collections.Generic;
using UnityEngine;
using EverScord.Augment;

namespace EverScord.Character
{
    public class CharacterStat : MonoBehaviour
    {
        private const float SPEED_FACTOR = 0.01f * 2f;
        [SerializeField] private float currentHealth;

        public Action OnCooldownBonusUpdated;
        private CharacterControl character;
        private IDictionary<StatType, StatBonus> bonusDict;
        private float maxHealth, moveSpeed, attack, critChance, defense, healthRegen;

        void Awake()
        {
            OnCooldownBonusUpdated -= UpdateSkillTimers;
            OnCooldownBonusUpdated += UpdateSkillTimers;
        }

        void Oestroy()
        {
            OnCooldownBonusUpdated -= UpdateSkillTimers;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4))
                DebugStats();
        }

        public void InitBaseStat(CharacterControl character)
        {
            bonusDict = new Dictionary<StatType, StatBonus>();
            int statCount = (int)StatType.END;

            for (int i = 0; i < statCount; i++)
                bonusDict[(StatType)i] = StatBonus.GetDefaultBonus();

            this.character = character;
            string tag = PlayerData.GetCharacterName(character.CharacterType);
            StatInfo info = StatData.StatInfoDict[tag];

            maxHealth           = info.health;
            healthRegen         = info.healthRegen;
            attack              = info.attack;
            defense             = info.defense;
            critChance          = info.critChance;
            moveSpeed           = info.speed * SPEED_FACTOR;

            currentHealth       = MaxHealth;

            if (character.CharacterJob == PlayerData.EJob.Healer)
                attack = info.supportAttack;
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
            OnCooldownBonusUpdated?.Invoke();
        }

        private void UpdateSkillTimers()
        {
            for (int i = 0; i < character.SkillList.Count; i++)
                character.SkillList[i].SkillAction.SetNewCooldown();
        }


        private void DebugStats()
        {
            Debug.Log("===============================================");
            Debug.Log($"Max HP: {MaxHealth}");
            Debug.Log($"SPEED: {Speed}");
            Debug.Log($"ATK: {Attack}");
            Debug.Log($"DEF: {Defense}");
            Debug.Log($"HP REGEN: {HealthRegen}");
            Debug.Log($"Alteration HP+: Additive: {bonusDict[StatType.HEAL_INCREASE].additive}, Mult: {bonusDict[StatType.HEAL_INCREASE].multiplicative}");
            Debug.Log($"Alteration COOLDOWN-:Additive: {bonusDict[StatType.COOLDOWN_DECREASE].additive}, Mult: {bonusDict[StatType.COOLDOWN_DECREASE].multiplicative}");
            Debug.Log($"Alteration RELOAD-: Additive: {bonusDict[StatType.RELOADSPEED_DECREASE].additive}, Mult: {bonusDict[StatType.RELOADSPEED_DECREASE].multiplicative}");
            Debug.Log($"Alteration SKILLDMG+: Additive: {bonusDict[StatType.SKILLDAMAGE_INCREASE].additive}, Mult: {bonusDict[StatType.SKILLDAMAGE_INCREASE].multiplicative}");
            Debug.Log("===============================================");
        }
    }
    
    public enum StatType
    {
        MAXHEALTH,
        SPEED,
        ATTACK,
        DEFENSE,
        HEALTH_REGEN,
        COOLDOWN_DECREASE,
        RELOADSPEED_DECREASE,
        SKILLDAMAGE_INCREASE,
        HEAL_INCREASE,
        END
    }
}
