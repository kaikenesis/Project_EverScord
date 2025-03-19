
using EverScord.Character;
using UnityEngine;

namespace EverScord
{
    public class DebugControl : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                CharacterControl.CurrentClientCharacter.IncreaseHP(CharacterControl.CurrentClientCharacter, 100);

            if (Input.GetKeyDown(KeyCode.F2))
                LevelControl.SyncChangeCurrentProgress(LevelControl.MaxProgress);

            if (Input.GetKeyDown(KeyCode.F3))
                CharacterControl.CurrentClientCharacter.DecreaseHP(1000);

            if (Input.GetKeyDown(KeyCode.F4))
                DebugStats();

            if (Input.GetKeyDown(KeyCode.F5))
                GameManager.Instance.debugMode = !GameManager.Instance.debugMode;

            if (Input.GetKeyDown(KeyCode.F6))
                GameManager.Instance.GameOverController.ShowGameover(true);
        }

        public void DebugStats()
        {
            CharacterControl character = CharacterControl.CurrentClientCharacter;
            CharacterStat stats = character.Stats; 

            Debug.Log($"{character.CharacterType.ToString()}===============================================");
            Debug.Log($"Max HP: {stats.MaxHealth}");
            Debug.Log($"SPEED: {stats.Speed}");
            Debug.Log($"ATK: {stats.Attack}");
            Debug.Log($"DEF: {stats.Defense}");
            Debug.Log($"HP REGEN: {stats.HealthRegen}");
            Debug.Log($"Alteration HP+: Additive: {stats.BonusDict[StatType.HEAL_INCREASE].additive}, Mult: {stats.BonusDict[StatType.HEAL_INCREASE].multiplicative}");
            Debug.Log($"Alteration COOLDOWN-:Additive: {stats.BonusDict[StatType.COOLDOWN_DECREASE].additive}, Mult: {stats.BonusDict[StatType.COOLDOWN_DECREASE].multiplicative}");
            Debug.Log($"Alteration RELOAD-: Additive: {stats.BonusDict[StatType.RELOADSPEED_DECREASE].additive}, Mult: {stats.BonusDict[StatType.RELOADSPEED_DECREASE].multiplicative}");
            Debug.Log($"Alteration SKILLDMG+: Additive: {stats.BonusDict[StatType.SKILLDAMAGE_INCREASE].additive}, Mult: {stats.BonusDict[StatType.SKILLDAMAGE_INCREASE].multiplicative}");
            Debug.Log($"{character.CharacterType.ToString()}===============================================");
        }
    }

}
