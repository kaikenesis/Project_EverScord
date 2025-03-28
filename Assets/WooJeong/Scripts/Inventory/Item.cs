using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType 
{ equip, consumable, other }

[CreateAssetMenu(menuName = "ScriptableObjects/Data/ItemData")]
public class Item : ScriptableObject
{
    public string ItemName;
    public Sprite Icon;   
    public string ItemDescription;
    public int MaxStackSize = 1;
    public bool IsStackable;
    public EItemType ItemType;

    public int Damage;
    public int Defense;
    public int HealAmount;

    public enum EItemType
    {
        Weapon,
        Armor,
        Consumable,
        Quest,
        Material
    }
}
