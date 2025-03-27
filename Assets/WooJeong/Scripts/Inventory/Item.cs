using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType 
{ equip, consumable, other }

[CreateAssetMenu(menuName = "ScriptableObjects/Data/ItemData")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;   
    public string itemDescription;
    public int maxStackSize = 1;
    public bool isStackable;
    public ItemType itemType;

    public int damage;
    public int defense;
    public int healAmount;

    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable,
        Quest,
        Material
    }
}
