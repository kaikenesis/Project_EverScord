using EverScord.Armor;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public IHelmet helmet { get; private set; }
    public IVest vest { get; private set; }
    public IShoes shoes { get; private set; }

    void Start()
    {
        helmet = new Helmet(10, 10, 10, 10, 10);
        vest = new Vest(10, 10, 10);
        shoes = new Shoes(10, 10, 10);

        Debug_Helmet();
    }

    public void SetHelmet(IHelmet newHelmet)
    {
        helmet = newHelmet;

        Debug_Helmet();
    }

    public void SetVest(IVest newVest)
    {
        vest = newVest;
    }

    public void SetShoes(IShoes newShoes)
    {
        shoes = newShoes;
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
