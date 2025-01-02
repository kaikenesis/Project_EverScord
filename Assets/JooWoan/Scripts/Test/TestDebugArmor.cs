using EverScord;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestDebugArmor : MonoBehaviour
{
    [SerializeField] private TestPlayer player;
    [SerializeField] private GameObject debugArmor;
    [SerializeField] private List<TextMeshProUGUI> helmetTexts, vestTexts, shoesTexts;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
            debugArmor.SetActive(!debugArmor.activeSelf);

        ShowStat();
    }

    private void ShowStat()
    {
        helmetTexts[0].text = $"평타 데미지: <color=yellow>{player.helmet.BasicAttackDamage.ToString("F2")}</color>";
        helmetTexts[1].text = $"스킬 데미지: <color=yellow>{player.helmet.SkillDamage.ToString("F2")}</color>";
        helmetTexts[2].text = $"평타 치유량: <color=yellow>{player.helmet.BasicHealAmount.ToString("F2")}</color>";
        helmetTexts[3].text = $"스킬 치유량: <color=yellow>{player.helmet.SkillHealAmount.ToString("F2")}</color>";
        helmetTexts[4].text = $"전체 치유량: <color=yellow>{player.helmet.AllroundHealAmount.ToString("F2")}</color>";

        vestTexts[0].text   = $"체력: <color=yellow>{player.vest.Health.ToString("F2")}</color>";
        vestTexts[1].text   = $"방어력: <color=yellow>{player.vest.Defense.ToString("F2")}</color>";
        vestTexts[2].text   = $"체력 회복량: <color=yellow>{player.vest.HealthRegeneration.ToString("F2")}</color>";

        shoesTexts[0].text  = $"이동속도: <color=yellow>{player.shoes.MoveSpeed.ToString("F2")}</color>";
        shoesTexts[1].text  = $"쿨타임: <color=yellow>{player.shoes.Cooldown.ToString("F2")}</color>";
        shoesTexts[2].text  = $"재장전 속도: <color=yellow>{player.shoes.ReloadSpeed.ToString("F2")}</color>";
    }
}
