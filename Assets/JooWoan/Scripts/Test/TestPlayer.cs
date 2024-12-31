using EverScord.Armor;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public IHelmet helmet { get; private set; }

    void Start()
    {
        helmet = new Helmet(10, 10, 10, 10, 10);
        Debug_Helmet();
    }

    public void SetHelmet(IHelmet newHelmet)
    {
        helmet = newHelmet;
        Debug_Helmet();
    }

    private void Debug_Helmet()
    {
        Debug.Log($"Basic attack damage : {helmet.BasicAttackDamage}");
        Debug.Log($"Skill damage : {helmet.SkillDamage}");
        Debug.Log($"Basic heal amount : {helmet.BasicHealAmount}");
        Debug.Log($"Skill heal amount : {helmet.SkillHealAmount}");
        Debug.Log($"All-round heal amount : {helmet.AllroundHealAmount}");
        Debug.Log("=========================================");
    }
}
