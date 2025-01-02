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
        helmetTexts[0].text = $"��Ÿ ������: <color=yellow>{player.helmet.BasicAttackDamage.ToString("F2")}</color>";
        helmetTexts[1].text = $"��ų ������: <color=yellow>{player.helmet.SkillDamage.ToString("F2")}</color>";
        helmetTexts[2].text = $"��Ÿ ġ����: <color=yellow>{player.helmet.BasicHealAmount.ToString("F2")}</color>";
        helmetTexts[3].text = $"��ų ġ����: <color=yellow>{player.helmet.SkillHealAmount.ToString("F2")}</color>";
        helmetTexts[4].text = $"��ü ġ����: <color=yellow>{player.helmet.AllroundHealAmount.ToString("F2")}</color>";

        vestTexts[0].text   = $"ü��: <color=yellow>{player.vest.Health.ToString("F2")}</color>";
        vestTexts[1].text   = $"����: <color=yellow>{player.vest.Defense.ToString("F2")}</color>";
        vestTexts[2].text   = $"ü�� ȸ����: <color=yellow>{player.vest.HealthRegeneration.ToString("F2")}</color>";

        shoesTexts[0].text  = $"�̵��ӵ�: <color=yellow>{player.shoes.MoveSpeed.ToString("F2")}</color>";
        shoesTexts[1].text  = $"��Ÿ��: <color=yellow>{player.shoes.Cooldown.ToString("F2")}</color>";
        shoesTexts[2].text  = $"������ �ӵ�: <color=yellow>{player.shoes.ReloadSpeed.ToString("F2")}</color>";
    }
}
